using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Pk2 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    public partial class DialogLogic : Form
    {
        public DelegateVddCallback VddCallback;
    
        private const int SAMPLE_ARRAY_LENGTH = 1024;
        private byte[] sampleArray = new byte[SAMPLE_ARRAY_LENGTH];
        private int lastZoomLevel = 1;
        private int lastSampleRate = 0;
        private int lastTrigPos = 50;
        private int lastTrigDelay = 0;
        private int firstSample = 0;
        private float[] sampleRates = new float[9] {1e-6F, 2e-6F, 4e-6F, 1e-5F, 2e-5F, 4e-5F, 1e-4F, 2e-4F, 1e-3F};
        private byte[] sampleFactorsPk2 = new byte[9] {0, 1, 3, 9, 19, 39, 99, 199, 255 /*999*/};
        private int[] sampleFactorsPk3 = new int[9] { 0, 16, 48, 144, 304, 624, 1584, 3184, 255 /*999*/};
        private Bitmap lastDrawnDisplay;
        private int cursor1Pos = 0;
        private int cursor2Pos = 0;
        private int postTrigCount = 1;
        private DialogTrigger trigDialog;
        private bool vddOn = false;
    
        public DialogLogic()
        {
            InitializeComponent();
            this.KeyPress += new KeyPressEventHandler(DialogLogic_KeyPress);
            
            for (int i = 0; i < SAMPLE_ARRAY_LENGTH; i++)
            { // init
                sampleArray[i] = 0;
            }
            
            initLogicIO();
            
            comboBoxCh1Trig.SelectedIndex = 0;
            comboBoxCh2Trig.SelectedIndex = 0;
            comboBoxCh3Trig.SelectedIndex = 0;
            comboBoxSampleRate.SelectedIndex = 0;
            labelCursor1Val.Text = "0 us";
            labelCursor2Val.Text = "0 us";
            labelCursorDeltaVal.Text = "0 us";

            updateDisplay();
        }
        
        public bool getModeAnalyzer()
        {
            return radioButtonAnalyzer.Checked;
        }
        
        public void setModeAnalyzer()
        {
            radioButtonLogicIO.Checked = false;
            radioButtonAnalyzer.Checked = true;
        }
        
        public int getZoom()
        {
            return lastZoomLevel;
        }
        
        public void setZoom(int zoom)
        {
            lastZoomLevel = zoom;
            if (zoom != 1)
            {
                radioButtonZoom1x.Checked = false;
                if (zoom == 0)
                    radioButtonZoom05.Checked = true;
                else if (zoom == 2)
                    radioButton2x.Checked = true;
                else if (zoom == 3)
                    radioButton4x.Checked = true;
                updateDisplay();
            }
        }
        
        public int getCh1Trigger()
        {
            return comboBoxCh1Trig.SelectedIndex;
        }
        
        public void setCh1Trigger(int trig)
        {
            comboBoxCh1Trig.SelectedIndex = trig;
        }

        public int getCh2Trigger()
        {
            return comboBoxCh2Trig.SelectedIndex;
        }

        public void setCh2Trigger(int trig)
        {
            comboBoxCh2Trig.SelectedIndex = trig;
        }

        public int getCh3Trigger()
        {
            return comboBoxCh3Trig.SelectedIndex;
        }

        public void setCh3Trigger(int trig)
        {
            comboBoxCh3Trig.SelectedIndex = trig;
        }

        public int getTrigCount()
        {
            return Int32.Parse(textBox1.Text);
        }

        public void setTrigCount(int count)
        {
            textBox1.Text = count.ToString();
        }

        public int getSampleRate()
        {
            return comboBoxSampleRate.SelectedIndex;
        }

        public void setSampleRate(int rate)
        {
            comboBoxSampleRate.SelectedIndex = rate;
        }

        public int getTriggerPosition()
        {
            int trigpos = 0;
            
            if (radioButtonTrigMid.Checked)
                trigpos = 1;
            else if (radioButtonTrigEnd.Checked)
                trigpos = 2;
            else if (radioButtonTrigDly1.Checked)
                trigpos = 3;
            else if (radioButtonTrigDly2.Checked)
                trigpos = 4;
            else if (radioButtonTrigDly3.Checked)
                trigpos = 5;
        
            return trigpos;
        }

        public void setTriggerPosition(int trigpos)
        {
            if (trigpos > 0)
            {
                radioButtonTrigStart.Checked = false;
                if (trigpos == 1)
                    radioButtonTrigMid.Checked = true;
                else if (trigpos == 2)
                    radioButtonTrigEnd.Checked = true;
                else if (trigpos == 3)
                    radioButtonTrigDly1.Checked = true;
                else if (trigpos == 4)
                    radioButtonTrigDly2.Checked = true;
                else if (trigpos == 5)
                    radioButtonTrigDly3.Checked = true;
                updateDisplay();
            }
        }        

        public bool getCursorsEnabled()
        {
            return checkBoxCursors.Checked;
        }

        public void setCursorsEnabled(bool enable)
        {
            checkBoxCursors.Checked = enable;
        }

        public int getXCursorPos()
        {
            return cursor1Pos;
        }

        public void setXCursorPos(int pos)
        {
            cursor1Pos = pos;
            updateDisplay();
        }

        public int getYCursorPos()
        {
            return cursor2Pos;
        }

        public void setYCursorPos(int pos)
        {
            cursor2Pos = pos;
            updateDisplay();
        }

        public void SetVddBox(bool enable, bool check)
        {
            checkBoxVDD.Enabled = enable;
            checkBoxVDD.Checked = check;
        }    
        
        private void updateDisplay()
        {
            // update signal display
            Bitmap display = drawSampleData(lastZoomLevel, lastTrigPos, firstSample);
            pictureBoxDisplay.Width = display.Width;
            if (!checkBoxCursors.Checked)
                pictureBoxDisplay.Image = display; // don't update the GUI if we're going to draw the cursors.
            lastDrawnDisplay = display;
            
            // Update cursors
            updateDisplayCursors();
            
            // update timescale
            float sampleTime = sampleRates[lastSampleRate];
            sampleTime *= 50; // 50 samples / div
            if (lastZoomLevel == 0)
                sampleTime *= 2;
            else if (lastZoomLevel == 2)
                sampleTime /= 2;
            else if (lastZoomLevel == 3)
                sampleTime /= 4;     
                
            string timeUnit = "s";
            if (sampleTime < .001F)
            {
                sampleTime *= 1e6F;
                timeUnit = "us";
            }
            else if (sampleTime < 1F)
            {
                sampleTime *= 1e3F;
                timeUnit = "ms";
            }   
            labelTimeScale.Text = string.Format("{0:G} ",  sampleTime) + timeUnit + " / Div";      
                
        }
        
        private Bitmap drawSampleData(int zoom, int triggerPos, int startPos)
        { // zoom 0 = 0.5x, 1 = 1x, 2=2x, 3 = 4x
            //int length = 1024;
            int width = 100;
            int zoomWidth = 1;
            if (zoom == 0)
            {
                //length = 512;
                triggerPos /= 2;
                zoomWidth = 0;
            }
            else if (zoom == 2)
            {
                //length = 2048;
                triggerPos *= 2;
                zoomWidth = 2;
            }
            else if (zoom == 3)
            {
                //length = 4096;
                triggerPos *= 4;
                zoomWidth = 4;
            }
                
            Bitmap sampleDisplay = getDisplayGraph(zoom);
            Graphics graphics = Graphics.FromImage(sampleDisplay);
            
            // draw trigger in Red
            SolidBrush trig = new SolidBrush(Color.Red);
            graphics.FillRectangle(trig, triggerPos, 0, 1, width);
            graphics.FillRectangle(trig, triggerPos-2, 4, 5, 2);
            graphics.FillRectangle(trig, triggerPos - 2, width - 5, 5, 2);
            if (lastTrigDelay > 0)
            {
                trig = new SolidBrush(Color.White);
                // draw triangle
                Point[] triangle = new Point[3];
                triangle[0] = new Point(triggerPos - 4, 0);
                triangle[1] = new Point(triggerPos - 4, 8);
                triangle[2] = new Point(triggerPos - 9, 4);
                graphics.FillPolygon(trig,triangle);
                float delayTime = sampleRates[lastSampleRate] * 1000 * lastTrigDelay;
                System.Drawing.Font labelFont = new Font(FontFamily.GenericSansSerif, 7F, FontStyle.Bold);
                string timeUnit = "s";
                if (delayTime < .001F)
                {
                    delayTime *= 1e6F;
                    timeUnit = "us";
                }
                else if (delayTime < 1F)
                {
                    delayTime *= 1e3F;
                    timeUnit = "ms";
                }
                graphics.DrawString(string.Format("DLY {0:G} ", delayTime)+ timeUnit, labelFont, trig, triggerPos+3, -2);
                labelFont.Dispose();
            }
            trig.Dispose();
            
            // draw data : 0-level 3 pixels wide, 1-level 1 pixels wide
            SolidBrush data = new SolidBrush(Color.LimeGreen);
            int displayPos = 0;
            byte lastSample = sampleArray[0];
            if (zoomWidth > 0)
            {
                for (int i = 0; i < SAMPLE_ARRAY_LENGTH; i++)
                { // array bit 2 = CH 3, array bit 1 = CH 2, array bit 0 = CH 0
                    // Zoom = 1x - 4x
                    // Ch 1 displays from y positions 10(1) to 29(0)
                    if (((sampleArray[i] & 0x1) ^ (lastSample & 0x1)) > 0)
                    { // data changed - draw vertical bar
                        graphics.FillRectangle(data, displayPos, 10, 1, 20); 
                        if (zoomWidth > 1)
                        {
                            if ((sampleArray[i] & 0x1) == 0)
                                graphics.FillRectangle(data, displayPos+1, 27, zoomWidth-1, 3); // 0
                            else
                                graphics.FillRectangle(data, displayPos+1, 10, zoomWidth-1, 1); // 1
                        }
                    }
                    else
                    { // single point
                        if ((sampleArray[i] & 0x1) == 0)
                            graphics.FillRectangle(data, displayPos, 27, zoomWidth, 3); // 0
                        else
                            graphics.FillRectangle(data, displayPos, 10, zoomWidth, 1); // 1
                    }
                    // Ch 2 displays from y positions 40(1) to 59(0)
                    if (((sampleArray[i] & 0x2) ^ (lastSample & 0x2)) > 0)
                    { // data changed - draw vertical bar
                        graphics.FillRectangle(data, displayPos, 40, 1, 20);
                        if (zoomWidth > 1)
                        {
                            if ((sampleArray[i] & 0x2) == 0)
                                graphics.FillRectangle(data, displayPos + 1, 57, zoomWidth - 1, 3); // 0
                            else
                                graphics.FillRectangle(data, displayPos + 1, 40, zoomWidth - 1, 1); // 1
                        }
                    }
                    else
                    { // single point
                        if ((sampleArray[i] & 0x2) == 0)
                            graphics.FillRectangle(data, displayPos, 57, zoomWidth, 3); // 0
                        else
                            graphics.FillRectangle(data, displayPos, 40, zoomWidth, 1); // 1
                    }
                    // Ch 3 displays from y positions 70(1) to 89(0)
                    if (((sampleArray[i] & 0x4) ^ (lastSample & 0x4)) > 0)
                    { // data changed - draw vertical bar
                        graphics.FillRectangle(data, displayPos, 70, 1, 20);
                        if (zoomWidth > 1)
                        {
                            if ((sampleArray[i] & 0x4) == 0)
                                graphics.FillRectangle(data, displayPos + 1, 87, zoomWidth - 1, 3); // 0
                            else
                                graphics.FillRectangle(data, displayPos + 1, 70, zoomWidth - 1, 1); // 1
                        }
                    }
                    else
                    { // single point
                        if ((sampleArray[i] & 0x4) == 0)
                            graphics.FillRectangle(data, displayPos, 87, zoomWidth, 3); // 0
                        else
                            graphics.FillRectangle(data, displayPos, 70, zoomWidth, 1); // 1
                    }    
                    
                    displayPos += zoomWidth;
                    lastSample = sampleArray[i];  
                }                                           
            }
            else
            { // zoomWidth = 0 : 05x
                for (int i = 0; i < SAMPLE_ARRAY_LENGTH; i+= 2)
                { // array bit 2 = CH 3, array bit 1 = CH 2, array bit 0 = CH 0
                    if ((((sampleArray[i] & 0x1) ^ (lastSample & 0x1)) > 0) || (((sampleArray[i+1] & 0x1) ^ (lastSample & 0x1)) > 0))
                    { // data changed - draw vertical bar
                        graphics.FillRectangle(data, displayPos, 10, 1, 20);
                    }
                    else
                    { // single point
                        if ((sampleArray[i] & 0x1) == 0)
                            graphics.FillRectangle(data, displayPos, 27, 1, 3); // 0
                        else
                            graphics.FillRectangle(data, displayPos, 10, 1, 1); // 1
                    }
                    // Ch 2 displays from y positions 40(1) to 59(0)
                    if ((((sampleArray[i] & 0x2) ^ (lastSample & 0x2)) > 0) || (((sampleArray[i+1] & 0x2) ^ (lastSample & 0x2)) > 0))
                    { // data changed - draw vertical bar
                        graphics.FillRectangle(data, displayPos, 40, 1, 20);
                    }
                    else
                    { // single point
                        if ((sampleArray[i] & 0x2) == 0)
                            graphics.FillRectangle(data, displayPos, 57, 1, 3); // 0
                        else
                            graphics.FillRectangle(data, displayPos, 40, 1, 1); // 1
                    }
                    // Ch 3 displays from y positions 70(1) to 89(0)
                    if ((((sampleArray[i] & 0x4) ^ (lastSample & 0x4)) > 0) || (((sampleArray[i+1] & 0x4) ^ (lastSample & 0x4)) > 0))
                    { // data changed - draw vertical bar
                        graphics.FillRectangle(data, displayPos, 70, 1, 20);
                    }
                    else
                    { // single point
                        if ((sampleArray[i] & 0x4) == 0)
                            graphics.FillRectangle(data, displayPos, 87, 1, 3); // 0
                        else
                            graphics.FillRectangle(data, displayPos, 70, 1, 1); // 1
                    }
                    displayPos++;
                    lastSample = sampleArray[i+1];                     
                }
                
            }
            
            graphics.Dispose();
            data.Dispose();
            return sampleDisplay;
        }
        
        private Bitmap getDisplayGraph(int zoom)
        { // zoom 0 = 0.5x, 1 = 1x, 2=2x, 3 = 4x
            int length = 1024;
            int width = 100;
            if (zoom == 0)
                length = 512;
            else if (zoom == 2)
                length = 2048;
            else if (zoom == 3)
                length = 4096;

            Bitmap gridmap = new Bitmap(length, width, PixelFormat.Format16bppRgb555);
            Graphics graphics = Graphics.FromImage(gridmap);
            SolidBrush brush = new SolidBrush(Color.Black);
            graphics.FillRectangle(brush, 0, 0, length, width);
            brush.Dispose();
            
            SolidBrush brushGrid = new SolidBrush(Color.DarkGray);
            
            // draw grid marks
            for (int i = 0; i < (length - 50); i+=50)
            {
             // draw long mark every 50 pixels
             //graphics.FillRectangle(brushGrid, i, width-7, 1, 7);
                graphics.FillRectangle(brushGrid, i, 0, 1, width);
             //graphics.FillRectangle(brushGrid, i, 0, 1, 7);
             
             // short marks inbetween
             graphics.FillRectangle(brushGrid, i + 10, width - 7, 1, 7);
             graphics.FillRectangle(brushGrid, i + 20, width - 7, 1, 7);
             graphics.FillRectangle(brushGrid, i + 30, width - 7, 1, 7);
             graphics.FillRectangle(brushGrid, i + 40, width - 7, 1, 7);
             graphics.FillRectangle(brushGrid, i + 10, 0, 1, 7);
             graphics.FillRectangle(brushGrid, i + 20, 0, 1, 7);
             graphics.FillRectangle(brushGrid, i + 30, 0, 1, 7);
             graphics.FillRectangle(brushGrid, i + 40, 0, 1, 7);
            }
            
            // fill in the last bit
            int lastBit = ((length - 50) / 50) + 1;
            lastBit *= 50;
            
            if (lastBit < length)
                graphics.FillRectangle(brushGrid, lastBit, 0, 1, width);
            lastBit += 10;
            if (lastBit < length)
            {
                graphics.FillRectangle(brushGrid, lastBit, width - 7, 1, 7);
                graphics.FillRectangle(brushGrid, lastBit, 0, 1, 7);
            }
            lastBit += 10;
            if (lastBit < length)
            {
                graphics.FillRectangle(brushGrid, lastBit, width - 7, 1, 7);
                graphics.FillRectangle(brushGrid, lastBit, 0, 1, 7);
            }
            lastBit += 10;
            if (lastBit < length)
            {
                graphics.FillRectangle(brushGrid, lastBit, width - 7, 1, 7);
                graphics.FillRectangle(brushGrid, lastBit, 0, 1, 7);
            }
            lastBit += 10;
            if (lastBit < length)
            {
                graphics.FillRectangle(brushGrid, lastBit, width - 7, 1, 7);
                graphics.FillRectangle(brushGrid, lastBit, 0, 1, 7);
            }                        
            
            brushGrid.Dispose();
            graphics.Dispose();
            
            return gridmap;
        }

        private void radioButtonZoom05_Click(object sender, EventArgs e)
        {
            if (radioButtonZoom05.Checked)
            {
                if (lastZoomLevel == 1)
                {
                    cursor1Pos /= 2;
                    cursor2Pos /= 2;
                }
                else if (lastZoomLevel == 2)
                {
                    cursor1Pos /= 4;
                    cursor2Pos /= 4;
                }
                else if (lastZoomLevel == 3)
                {
                    cursor1Pos /= 8;
                    cursor2Pos /= 8;
                }
                lastZoomLevel = 0;
            }
            else if (radioButtonZoom1x.Checked)
            {
                if (lastZoomLevel == 0)
                {
                    cursor1Pos *= 2;
                    cursor2Pos *= 2;
                }
                else if (lastZoomLevel == 2)
                {
                    cursor1Pos /= 2;
                    cursor2Pos /= 2;
                }
                else if (lastZoomLevel == 3)
                {
                    cursor1Pos /= 4;
                    cursor2Pos /= 4;
                }            
                lastZoomLevel = 1;
            }
            else if (radioButton2x.Checked)
            {
                if (lastZoomLevel == 0)
                {
                    cursor1Pos *= 4;
                    cursor2Pos *= 4;
                }
                else if (lastZoomLevel == 1)
                {
                    cursor1Pos *= 2;
                    cursor2Pos *= 2;
                }
                else if (lastZoomLevel == 3)
                {
                    cursor1Pos /= 2;
                    cursor2Pos /= 2;
                }            
                lastZoomLevel = 2;
            }
            else if (radioButton4x.Checked)
            {
                if (lastZoomLevel == 0)
                {
                    cursor1Pos *= 8;
                    cursor2Pos *= 8;
                }
                else if (lastZoomLevel == 1)
                {
                    cursor1Pos *= 4;
                    cursor2Pos *= 4;
                }
                else if (lastZoomLevel == 2)
                {
                    cursor1Pos *= 2;
                    cursor2Pos *= 2;
                }            
                lastZoomLevel = 3;
            }
            updateDisplay();
        }

        private void checkBoxCursors_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCursors.Checked)
            {
                labelCursor1.Enabled = true;
                labelCursor1Val.Enabled = true;
                labelCursor2.Enabled = true;
                labelCursor2Val.Enabled = true;
                labelCursorDelta.Enabled = true;
                labelCursorDeltaVal.Enabled = true;
                updateDisplay(); // show cursors
            }
            else
            {
                labelCursor1.Enabled = false;
                labelCursor1Val.Enabled = false;
                labelCursor2.Enabled = false;
                labelCursor2Val.Enabled = false;
                labelCursorDelta.Enabled = false;
                labelCursorDeltaVal.Enabled = false;
                updateDisplay(); // delete cursors
            }

        }

        private void pictureBoxDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkBoxCursors.Checked)
            {
                if (e.Button == MouseButtons.Left)
                { // x curs
                    cursor1Pos = e.X;
                }
                else if (e.Button == MouseButtons.Right)
                { // y curs
                    cursor2Pos = e.X;
                }
            }
            updateDisplayCursors();
        }
        
        private void updateDisplayCursors()
        {
            if (!checkBoxCursors.Checked)
                return; // nothing to do
                
            int width = lastDrawnDisplay.Height;
            int cursorWidth = 1;
            int cursorXPos = cursor1Pos;
            int cursorYPos = cursor2Pos;
            if (lastZoomLevel == 0)
            {
                cursorXPos *= 2;
                cursorYPos *= 2;
            }
            else if (lastZoomLevel == 2)
            {
                cursorWidth = 2;
                cursorXPos /= 2;
                cursorYPos /= 2;
                cursor1Pos -= (cursor1Pos % 2);
                cursor2Pos -= (cursor2Pos % 2);
            }
            else if (lastZoomLevel == 3)
            {
                cursorWidth = 4;
                cursorXPos /= 4;
                cursorYPos /= 4;
                cursor1Pos -= (cursor1Pos % 4);
                cursor2Pos -= (cursor2Pos % 4);
            }            
            // draw cursors
            Bitmap localBitmap = (Bitmap)lastDrawnDisplay.Clone();
            Graphics graphics = Graphics.FromImage(localBitmap);
            System.Drawing.Font labelFont = new Font(FontFamily.GenericSansSerif, 7F);
            
            // draw X cursor 1
            SolidBrush brush = new SolidBrush(Color.DodgerBlue);
            graphics.FillRectangle(brush, cursor1Pos, 0, cursorWidth, width);
            graphics.DrawString("X", labelFont, brush, (float)(cursor1Pos - 10), 29F);
            // update time display
            float cursorXtime = (float)(cursorXPos - lastTrigPos) * sampleRates[lastSampleRate];
            cursorXtime += (float)(sampleRates[lastSampleRate] * 1000 * lastTrigDelay);
            string timeUnit = "s";
            int roundDigits = 3;
            if (Math.Abs(cursorXtime) < .001F)
            {
                cursorXtime *= 1e6F;
                timeUnit = "us";
                roundDigits = 0;
            }
            else if (Math.Abs(cursorXtime) < 1F)
            {
                cursorXtime *= 1e3F;
                timeUnit = "ms";
            }
            labelCursor1Val.Text = string.Format("{0:G} ", Math.Round((decimal)cursorXtime, roundDigits)) + timeUnit;

            // draw Y cursor 2
            brush = new SolidBrush(Color.MediumOrchid);
            graphics.FillRectangle(brush, cursor2Pos, 0, cursorWidth, width);
            graphics.DrawString("Y", labelFont, brush, (float)(cursor2Pos + cursorWidth + 2), 29F);
            // update time display
            float cursorYtime = (float)(cursorYPos - lastTrigPos) * sampleRates[lastSampleRate];
            cursorYtime += (float)(sampleRates[lastSampleRate] * 1000 * lastTrigDelay);
            timeUnit = "s";
            roundDigits = 3;
            if (Math.Abs(cursorYtime) < .001F)
            {
                cursorYtime *= 1e6F;
                timeUnit = "us";
                roundDigits = 0;
            }
            else if (Math.Abs(cursorYtime) < 1F)
            {
                cursorYtime *= 1e3F;
                timeUnit = "ms";
            }
            labelCursor2Val.Text = string.Format("{0:G} ", Math.Round((decimal)cursorYtime, 3)) + timeUnit;            
            
            pictureBoxDisplay.Image = localBitmap;
            
            // update delta time
            cursorYtime = (float)(cursorYPos - cursorXPos) * sampleRates[lastSampleRate];
            float freq = 0F;
            if (Math.Abs(cursorYtime) > 0)
                freq = Math.Abs(1F / cursorYtime);
            timeUnit = "s";
            if (Math.Abs(cursorYtime) < .001F)
            {
                cursorYtime *= 1e6F;
                timeUnit = "us";
            }
            else if (Math.Abs(cursorYtime) < 1F)
            {
                cursorYtime *= 1e3F;
                timeUnit = "ms";
            }
            string freqUnit = "Hz)";
            if (freq >= 10000)
            {
                freq /= 1000;
                freqUnit = "kHz)";
            }
            labelCursorDeltaVal.Text = string.Format("{0:G} ", Math.Round((decimal)cursorYtime, 2)) + timeUnit 
                                        + string.Format(" ({0:G} ", Math.Round((decimal)freq,2)) + freqUnit;
            
            brush.Dispose();
            graphics.Dispose();
            
        }

        private void comboBoxCh1Trig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBoxCh1Trig.SelectedIndex > 2) && (comboBoxCh2Trig.SelectedIndex > 2))
            { // both edges
                 if (comboBoxCh1Trig.SelectedIndex != comboBoxCh2Trig.SelectedIndex)
                 {// different directions
                     MessageBox.Show("If more than one Channel is set to\nedge detect, all Channel edges must\nbe in the same direction.\n\n(Rising or Falling)");
                     comboBoxCh1Trig.SelectedIndex = 0;
                 }
            }
            if ((comboBoxCh1Trig.SelectedIndex > 2) && (comboBoxCh3Trig.SelectedIndex > 2))
            { // both edges
                if (comboBoxCh1Trig.SelectedIndex != comboBoxCh3Trig.SelectedIndex)
                {// different directions
                    MessageBox.Show("If more than one Channel is set to\nedge detect, all Channel edges must\nbe in the same direction.\n\n(Rising or Falling)");
                    comboBoxCh1Trig.SelectedIndex = 0;
                }
            }            
        }

        private void comboBoxCh2Trig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBoxCh1Trig.SelectedIndex > 2) && (comboBoxCh2Trig.SelectedIndex > 2))
            { // both edges
                if (comboBoxCh1Trig.SelectedIndex != comboBoxCh2Trig.SelectedIndex)
                {// different directions
                    MessageBox.Show("If more than one Channel is set to\nedge detect, all Channel edges must\nbe in the same direction.\n\n(Rising or Falling)");
                    comboBoxCh2Trig.SelectedIndex = 0;
                }
            }
            if ((comboBoxCh2Trig.SelectedIndex > 2) && (comboBoxCh3Trig.SelectedIndex > 2))
            { // both edges
                if (comboBoxCh2Trig.SelectedIndex != comboBoxCh3Trig.SelectedIndex)
                {// different directions
                    MessageBox.Show("If more than one Channel is set to\nedge detect, all Channel edges must\nbe in the same direction.\n\n(Rising or Falling)");
                    comboBoxCh2Trig.SelectedIndex = 0;
                }
            } 
        }

        private void comboBoxCh3Trig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((comboBoxCh1Trig.SelectedIndex > 2) && (comboBoxCh3Trig.SelectedIndex > 2))
            { // both edges
                if (comboBoxCh1Trig.SelectedIndex != comboBoxCh3Trig.SelectedIndex)
                {// different directions
                    MessageBox.Show("If more than one Channel is set to\nedge detect, all Channel edges must\nbe in the same direction.\n\n(Rising or Falling)");
                    comboBoxCh3Trig.SelectedIndex = 0;
                }
            }
            if ((comboBoxCh2Trig.SelectedIndex > 2) && (comboBoxCh3Trig.SelectedIndex > 2))
            { // both edges
                if (comboBoxCh2Trig.SelectedIndex != comboBoxCh3Trig.SelectedIndex)
                {// different directions
                    MessageBox.Show("If more than one Channel is set to\nedge detect, all Channel edges must\nbe in the same direction.\n\n(Rising or Falling)");
                    comboBoxCh3Trig.SelectedIndex = 0;
                }
            } 
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            uint trigCount = UInt32.Parse(textBox1.Text);
            if (trigCount == 0)
                trigCount = 1;
            if (trigCount > 256)
                trigCount = 256;
            textBox1.Text = trigCount.ToString();
        }

        private void comboBoxSampleRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            float aliasFreq = sampleRates[comboBoxSampleRate.SelectedIndex];
            aliasFreq = 0.5F / aliasFreq;
            string freqUnit = "Hz";
            if (aliasFreq >= 10000)
            {
                aliasFreq /= 1000;
                freqUnit = "kHz";
            }
            labelAliasFreq.Text = string.Format("NOTE: Signals greater than {0:G} ", Math.Round((decimal)aliasFreq, 1)) + freqUnit
                                        + " will alias.";
        }


        private void buttonRun_Click(object sender, EventArgs e)
        {
            if ((comboBoxCh1Trig.SelectedIndex == 0) && (comboBoxCh2Trig.SelectedIndex == 0)
                && (comboBoxCh3Trig.SelectedIndex == 0))
            {
                MessageBox.Show("At least one trigger condition\n must be defined.\n\nAll are set to Don't Care.", Pk2.ToolName + " Logic Tool");
                return;
            }
            
            // calculate parameters
            byte risingEdge = 1;
            if ((comboBoxCh1Trig.SelectedIndex == 4) || (comboBoxCh2Trig.SelectedIndex == 4)
                || (comboBoxCh3Trig.SelectedIndex == 4))
            { // any are falling edge
                risingEdge = 0;
            }
            byte trigMask = 0;
            if (comboBoxCh1Trig.SelectedIndex > 0)
                trigMask |= 0x04;
            if (comboBoxCh2Trig.SelectedIndex > 0)
                trigMask |= 0x08;                
            if (comboBoxCh3Trig.SelectedIndex > 0)
                trigMask |= 0x10;
            byte trigStates = 0;
            if ((comboBoxCh1Trig.SelectedIndex == 1) || (comboBoxCh1Trig.SelectedIndex == 3))
                trigStates |= 0x04;
            if ((comboBoxCh2Trig.SelectedIndex == 1) || (comboBoxCh2Trig.SelectedIndex == 3))
                trigStates |= 0x08;
            if ((comboBoxCh3Trig.SelectedIndex == 1) || (comboBoxCh3Trig.SelectedIndex == 3))
                trigStates |= 0x10;
            byte edgeMask = 0;
            if ((comboBoxCh1Trig.SelectedIndex == 4) || (comboBoxCh1Trig.SelectedIndex == 3))
                edgeMask |= 0x04;
            if ((comboBoxCh2Trig.SelectedIndex == 4) || (comboBoxCh2Trig.SelectedIndex == 3))
                edgeMask |= 0x08;                
            if ((comboBoxCh3Trig.SelectedIndex == 4) || (comboBoxCh3Trig.SelectedIndex == 3))
                edgeMask |= 0x10;
            byte trigCount = Byte.Parse(textBox1.Text);
            postTrigCount = 2;
            if (radioButtonTrigStart.Checked)
                postTrigCount = 973; // 51 samples in
            else if (radioButtonTrigMid.Checked)
                postTrigCount = 523; // 501 samples in
            else if (radioButtonTrigEnd.Checked)
                postTrigCount = 73;  // 951 samples in
            else if (radioButtonTrigDly1.Checked)
                postTrigCount = 1973;  // 951 + 1000 samples in      
            else if (radioButtonTrigDly2.Checked)
                postTrigCount = 2973;  // 951 + 2000 samples in  
            else if (radioButtonTrigDly3.Checked)
                postTrigCount = 3973;  // 951 + 3000 samples in                          
                
            // 
            trigDialog = new DialogTrigger();
            this.AddOwnedForm(trigDialog);
            trigDialog.Show();

            byte[] commandArrayp = new byte[10];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.LOGIC_ANALYZER_GO;
            commandArrayp[commOffSet++] = risingEdge;
            commandArrayp[commOffSet++] = trigMask;
            commandArrayp[commOffSet++] = trigStates;                
            commandArrayp[commOffSet++] = edgeMask;
            commandArrayp[commOffSet++] = trigCount;     
            commandArrayp[commOffSet++] = (byte)((postTrigCount - 1) & 0xFF);
            commandArrayp[commOffSet++] = (byte)(((postTrigCount - 1) >> 8) & 0xFF);
            if (Pk2.isPK3)
            {
                commandArrayp[commOffSet++] = (byte)(sampleFactorsPk3[comboBoxSampleRate.SelectedIndex] & 0xFF);
                commandArrayp[commOffSet++] = (byte)((sampleFactorsPk3[comboBoxSampleRate.SelectedIndex] >> 8) & 0xFF);
            }
            else
            {
                commandArrayp[commOffSet++] = sampleFactorsPk2[comboBoxSampleRate.SelectedIndex];
            }
            Pk2.writeUSB(commandArrayp);
            timerRun.Enabled = true;
        }

        private void timerRun_Tick(object sender, EventArgs e)
        {
            timerRun.Enabled = false;
            // bool ret = Pk2.readUSB();  // will wait here until trigger
            bool ret = false;
            while (ret == false)
            {
                ret = Pk2.readUSBNoDisconnect();  // will wait here until trigger
            }
            Thread.Sleep(250);
            this.RemoveOwnedForm(trigDialog);
            trigDialog.Close();
            if (!ret)
            {
                return;
            }
            
            int trigAddr = Pk2.Usb_read_array[1] + (Pk2.Usb_read_array[2] * 0x100);
            if ( (!Pk2.isPK3 && ((trigAddr & 0x4000) > 0)) || (Pk2.isPK3 && (trigAddr == 0xFFFF)) )
            { // aborted
                // keep existing data
                return;
            }
            
            // update variables for display
            lastSampleRate = comboBoxSampleRate.SelectedIndex;  
            
            bool upperData = ((trigAddr & 0x8000) > 0);

            int trigAddrMask, trigAddrStart;

            // some differences between Pk2 and Pk3 in addresses allocated for analyzer buffer
            if (Pk2.isPK3)
            {
                trigAddrMask = 0x7FFF;
                trigAddrStart = 0x4000;
            }
            else
            {
                trigAddrMask = 0xFFF;
                trigAddrStart = 0x600;
            }


            trigAddr &= trigAddrMask;
            trigAddr++;
            if (trigAddr == (trigAddrStart + 0x200))
                trigAddr = trigAddrStart;
            trigAddr -= trigAddrStart;
            
            // get data samples
            byte[] dataArray = new byte[512];
            byte[] commandArrayp = new byte[3];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.COPY_RAM_UPLOAD;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = (byte)(trigAddrStart>>8);
            Pk2.writeUSB(commandArrayp);
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, dataArray, 0, KONST.USB_REPORTLENGTH);
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, dataArray, KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.COPY_RAM_UPLOAD;
            commandArrayp[commOffSet++] = 0x80;
            commandArrayp[commOffSet++] = (byte)(trigAddrStart >> 8);
            Pk2.writeUSB(commandArrayp);
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, dataArray, 2 * KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, dataArray, 3 * KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.COPY_RAM_UPLOAD;
            commandArrayp[commOffSet++] = 0x00;
            commandArrayp[commOffSet++] = (byte)((trigAddrStart >> 8) + 1);
            Pk2.writeUSB(commandArrayp);
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, dataArray, 4 * KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, dataArray, 5 * KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
            commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.COPY_RAM_UPLOAD;
            commandArrayp[commOffSet++] = 0x80;
            commandArrayp[commOffSet++] = (byte)((trigAddrStart >> 8) + 1);
            Pk2.writeUSB(commandArrayp);
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, dataArray, 6 * KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);
            Pk2.UploadDataNoLen();
            Array.Copy(Pk2.Usb_read_array, 1, dataArray, 7 * KONST.USB_REPORTLENGTH, KONST.USB_REPORTLENGTH);                      
            
            lastTrigPos = 1023 - (postTrigCount % 1000);
            lastTrigDelay = postTrigCount/1000;
            // so data starts at
            trigAddr += ((lastTrigPos/2) + (postTrigCount/1000)*12);
            if ((lastTrigPos % 2) > 0)
            {
                upperData = !upperData;
                if (upperData)
                    trigAddr++;
            }
            trigAddr %= 512;
            
            // put them into the sampleArray
            for (int i = 0; i < sampleArray.Length; i++)
            {
                byte sample = dataArray[trigAddr];
                if (upperData)
                {
                    trigAddr--;
                    if (trigAddr < 0)
                        trigAddr += 512;
                    sample = (byte)((sample >> 4) + (sample << 4));
                }
                sample &= 0x1C;
                sampleArray[i] = (byte)(sample >> 2);
                upperData = !upperData;
            }
            // sometimes first sample seems bogus. Backfill with 2nd sample until fixed.
            sampleArray[0] = sampleArray[1];
            
            updateDisplay();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Bitmap localBitmap = (Bitmap)pictureBoxDisplay.Image.Clone();
            Graphics graphics = Graphics.FromImage(localBitmap);
            System.Drawing.Font labelFont = new Font(FontFamily.GenericSansSerif, 7F, FontStyle.Bold);

            SolidBrush brush = new SolidBrush(Color.White);
            graphics.DrawString(labelTimeScale.Text, labelFont, brush, 5F, 88F);
            if (checkBoxCursors.Checked)
            {
                graphics.DrawString("X="+labelCursor1Val.Text, labelFont, brush, 100F, 88F);
                graphics.DrawString("Y=" + labelCursor2Val.Text, labelFont, brush, 200F, 88F);
            }
            
            saveFileDialogDisplay.ShowDialog();
            try
            {
                localBitmap.Save(saveFileDialogDisplay.FileName);
            }
            catch
            {
            }
            
            graphics.Dispose();
            brush.Dispose();
            labelFont.Dispose();
            localBitmap.Dispose();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            checkBoxIOEnable.Checked = false;
            this.Close();
        }

        private void radioButtonAnalyzer_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonLogicIO.Checked)
            {
                panelLogicIO.Visible = true;
                panelAnalyzer.Visible = false;
            }
            else
            {
                panelLogicIO.Visible = false;
                checkBoxIOEnable.Checked = false;
                panelAnalyzer.Visible = true;
            }
        }
        
        private void initLogicIO()
        {
            radioButtonPin4In.Checked = true;
            radioButtonPin5In.Checked = true;
            radioButtonPin6In.Checked = true;
            
            textBoxPin1Out.Enabled = true;
            textBoxPin1Out.Text = "0";
            textBoxPin1Out.BackColor = Color.DarkRed;
            labelOut1Click.Visible = true;
            
            textBoxPin4In.Enabled = true;
            textBoxPin4In.Text = "0";
            textBoxPin4In.BackColor = Color.DarkBlue;

            textBoxPin4Out.Enabled = false;
            textBoxPin4Out.Text = "0";
            textBoxPin4Out.BackColor = SystemColors.Control;
            labelOut4Click.Visible = false;

            textBoxPin5In.Enabled = true;
            textBoxPin5In.Text = "0";
            textBoxPin5In.BackColor = Color.DarkBlue;

            textBoxPin5Out.Enabled = false;
            textBoxPin5Out.Text = "0";
            textBoxPin5Out.BackColor = SystemColors.Control;
            labelOut5Click.Visible = false;

            textBoxPin6In.Enabled = true;
            textBoxPin6In.Text = "0";
            textBoxPin6In.BackColor = Color.DarkBlue;

            textBoxPin6Out.Enabled = false;
            textBoxPin6Out.Text = "0";
            textBoxPin6Out.BackColor = SystemColors.Control;
            labelOut6Click.Visible = false;                    
            
        }

        private void textBoxPin1Out_Click(object sender, EventArgs e)
        {
            pinOut(textBoxPin1Out);
        }

        private void textBoxPin4Out_Click(object sender, EventArgs e)
        {
            pinOut(textBoxPin4Out);
        }

        private void textBoxPin5Out_Click(object sender, EventArgs e)
        {
            pinOut(textBoxPin5Out);
        }

        private void textBoxPin6Out_Click(object sender, EventArgs e)
        {
            pinOut(textBoxPin6Out);
        }
        
        private void pinOut(TextBox textBoxObject)
        {
            if (checkBoxIOEnable.Checked)
            {
                if (textBoxObject.Enabled)
                {
                    if (textBoxObject.Text == "0")
                    {
                        textBoxObject.Text = "1";
                        textBoxObject.BackColor = Color.Red;
                    }
                    else
                    {
                        textBoxObject.Text = "0";
                        textBoxObject.BackColor = Color.DarkRed;
                    }
                    updateOutputs();                   
                }
            }
            else
            {
                MessageBox.Show("Click the 'Enable IO' button\n to use the Logic IO.", Pk2.ToolName + " Logic Tool");
            }
        }        

        private void checkBoxIOEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxIOEnable.Checked)
            {
                if (!initLogicPins())
                {
                    MessageBox.Show("No valid voltage detected on\n" + Pk2.ToolName + " VDD pin.\n\nA valid voltage (2.5V to 5.0V)\nis required for the Logic IO.", Pk2.ToolName + " Logic Tool");
                    checkBoxIOEnable.Checked = false;
                    return;
                }
                if (Pk2.PowerStatus() == Constants.PICkit2PWR.vdd_on)
                    vddOn = true;
                else
                    vddOn = false;
                radioButtonPin4In.Enabled = true;
                radioButtonPin4Out.Enabled = true;
                radioButtonPin5In.Enabled = true;
                radioButtonPin5Out.Enabled = true;
                radioButtonPin6In.Enabled = true;
                radioButtonPin6Out.Enabled = true;
                updateOutputs();
                timerIO.Enabled = true;
            }
            else
            {
                timerIO.Enabled = false;
                radioButtonPin4In.Enabled = false;
                radioButtonPin4Out.Enabled = false;
                radioButtonPin5In.Enabled = false;
                radioButtonPin5Out.Enabled = false;
                radioButtonPin6In.Enabled = false;
                radioButtonPin6Out.Enabled = false;
                exitLogicIO();
            }
        }

        private void radioButtonPin4Out_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPin4Out.Checked)
            {
                textBoxPin4In.Text = "";
                textBoxPin4In.BackColor = SystemColors.Control;
                textBoxPin4In.Enabled = false;
                
                textBoxPin4Out.Enabled = true;
                if (textBoxPin4Out.Text == "0")
                    textBoxPin4Out.BackColor = Color.DarkRed;
                else
                    textBoxPin4Out.BackColor = Color.Red;
                labelOut4Click.Visible = true;
            }
            else
            {
                textBoxPin4In.Enabled = true;
                textBoxPin4In.Text = "0";
                textBoxPin4In.BackColor = Color.DarkBlue;

                textBoxPin4Out.Enabled = false;
                textBoxPin4Out.BackColor = SystemColors.Control;
                labelOut4Click.Visible = false;
            }
            updateOutputs();
        }

        private void radioButtonPin5Out_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPin5Out.Checked)
            {
                textBoxPin5In.Text = "";
                textBoxPin5In.BackColor = SystemColors.Control;
                textBoxPin5In.Enabled = false;

                textBoxPin5Out.Enabled = true;
                if (textBoxPin5Out.Text == "0")
                    textBoxPin5Out.BackColor = Color.DarkRed;
                else
                    textBoxPin5Out.BackColor = Color.Red;
                labelOut5Click.Visible = true;                    
            }
            else
            {
                textBoxPin5In.Enabled = true;
                textBoxPin5In.Text = "0";
                textBoxPin5In.BackColor = Color.DarkBlue;

                textBoxPin5Out.Enabled = false;
                textBoxPin5Out.BackColor = SystemColors.Control;
                labelOut5Click.Visible = false;

            }
            updateOutputs();
        }

        private void radioButtonPin6Out_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPin6Out.Checked)
            {
                textBoxPin6In.Text = "";
                textBoxPin6In.BackColor = SystemColors.Control;
                textBoxPin6In.Enabled = false;

                textBoxPin6Out.Enabled = true;
                if (textBoxPin6Out.Text == "0")
                    textBoxPin6Out.BackColor = Color.DarkRed;
                else
                    textBoxPin6Out.BackColor = Color.Red;
                labelOut6Click.Visible = true;
            }
            else
            {
                textBoxPin6In.Enabled = true;
                textBoxPin6In.Text = "0";
                textBoxPin6In.BackColor = Color.DarkBlue;

                textBoxPin6Out.Enabled = false;
                textBoxPin6Out.BackColor = SystemColors.Control;
                labelOut6Click.Visible = false;

            }
            updateOutputs();
        }
        
        private bool initLogicPins()
        {
            float vdd = 0;
            float vpp = 0;
        
            if (Pk2.ReadPICkitVoltages(ref vdd, ref vpp))
            {
                if (vdd >= 2.5F)
                {
                    Pk2.SetVppVoltage(vdd, 0.7F);   // Set VPP to VDD
                    Pk2.SetVDDVoltage(vdd, 0.85F, false); // set VDD too, since VPP must be >= VDD.
                    // spin up VPP PWM and set all IO default states
                    byte[] commandArrayp = new byte[11];
                    int commOffSet = 0;
                    commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
                    commandArrayp[commOffSet++] = 9;
                    commandArrayp[commOffSet++] = KONST._VPP_OFF;
                    commandArrayp[commOffSet++] = KONST._MCLR_GND_ON;
                    commandArrayp[commOffSet++] = KONST._VPP_PWM_ON;
                    commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
                    commandArrayp[commOffSet++] = 0x03; // inputs
                    commandArrayp[commOffSet++] = KONST._SET_AUX;
                    commandArrayp[commOffSet++] = 0x01; // input
                    commandArrayp[commOffSet++] = KONST._DELAY_LONG;
                    commandArrayp[commOffSet++] = 20;
                   
                    return Pk2.writeUSB(commandArrayp);
                   
                }
            }
            return false;
        }
        
        private bool exitLogicIO()
        {
            // shut down IO & PWM
            byte[] commandArrayp = new byte[9];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 7;
            commandArrayp[commOffSet++] = KONST._VPP_OFF;
            commandArrayp[commOffSet++] = KONST._MCLR_GND_ON;
            commandArrayp[commOffSet++] = KONST._VPP_PWM_OFF;
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = 0x03; // inputs
            commandArrayp[commOffSet++] = KONST._SET_AUX;
            commandArrayp[commOffSet++] = 0x01; // input
           
            return Pk2.writeUSB(commandArrayp);
                    
        }
        
        private bool updateOutputs()
        {
            byte icsp = 0x03;
            byte aux = 0x01;
            
            // set directions
            if (radioButtonPin4Out.Checked)
            {
                icsp &= 0xFD;
                if (textBoxPin4Out.Text == "1")
                    icsp |= 0x08;
            }
            if (radioButtonPin5Out.Checked)
            {
                icsp &= 0xFE;
                if (textBoxPin5Out.Text == "1")
                    icsp |= 0x04;
            }
            if (radioButtonPin6Out.Checked)
            {
                aux = 0x00;
                if (textBoxPin6Out.Text == "1")
                    aux = 0x02;
            }
            byte[] commandArrayp = new byte[8];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 6;
            if (textBoxPin1Out.Text == "0")
            {
                commandArrayp[commOffSet++] = KONST._VPP_OFF;
                commandArrayp[commOffSet++] = KONST._MCLR_GND_ON;
            }
            else
            {
                commandArrayp[commOffSet++] = KONST._MCLR_GND_OFF;
                commandArrayp[commOffSet++] = KONST._VPP_ON;
            }
            commandArrayp[commOffSet++] = KONST._SET_ICSP_PINS;
            commandArrayp[commOffSet++] = icsp; 
            commandArrayp[commOffSet++] = KONST._SET_AUX;
            commandArrayp[commOffSet++] = aux; 

            return Pk2.writeUSB(commandArrayp);
        }


        public void DialogLogic_KeyPress(object sender, KeyPressEventArgs e)
        { // MUST set form property "KeyPreview" to True!
            if (panelLogicIO.Visible && checkBoxIOEnable.Checked)
            {
                if ((e.KeyChar == 'a') || (e.KeyChar == 'A'))
                    pinOut(textBoxPin1Out);
                else if ((e.KeyChar == 's') || (e.KeyChar == 'S'))
                    pinOut(textBoxPin4Out);
                else if ((e.KeyChar == 'd') || (e.KeyChar == 'D'))
                    pinOut(textBoxPin5Out);
                else if ((e.KeyChar == 'f') || (e.KeyChar == 'F'))
                    pinOut(textBoxPin6Out);
            }
        }

        private void timerIO_Tick(object sender, EventArgs e)
        {
            //check for power errors
            KONST.PICkit2PWR result = Pk2.PowerStatus();
            if ((result == KONST.PICkit2PWR.vdderror) || (result == KONST.PICkit2PWR.vddvpperrors))
            {
                MessageBox.Show(Pk2.ToolName + " VDD voltage level error.\nVDD shut off: Disabling IO", Pk2.ToolName + " Error");
                checkBoxIOEnable.Checked = false;
                return;
            }
            else if (result == KONST.PICkit2PWR.vpperror)
            {
                if (vddOn)
                {
                    MessageBox.Show("Voltage error on Pin 1:\nVDD was shut off.\n\nDisabling IO", Pk2.ToolName + " Error");
                    checkBoxIOEnable.Checked = false;
                    return;
                }
                else
                { // if external power, this is recoverable.
                    MessageBox.Show("Voltage error on Pin 1:\nState reset to '0'", Pk2.ToolName + " Error");
                    textBoxPin1Out.Text = "0";
                    textBoxPin1Out.BackColor = Color.DarkRed;
                }
            }
        
            // read pin states
            byte[] commandArrayp = new byte[5];
            int commOffSet = 0;
            commandArrayp[commOffSet++] = KONST.EXECUTE_SCRIPT;
            commandArrayp[commOffSet++] = 2;
            commandArrayp[commOffSet++] = KONST._ICSP_STATES_BUFFER;
            commandArrayp[commOffSet++] = KONST._AUX_STATE_BUFFER;
            commandArrayp[commOffSet++] = KONST.UPLOAD_DATA;

            Pk2.writeUSB(commandArrayp);
            Pk2.readUSB();
            
            if ((Pk2.Usb_read_array[2] & 0x02) > 0)
                updateInputBox(textBoxPin4In, "1");
            else
                updateInputBox(textBoxPin4In, "0");

            if ((Pk2.Usb_read_array[2] & 0x01) > 0)
                updateInputBox(textBoxPin5In, "1");
            else
                updateInputBox(textBoxPin5In, "0");

            if ((Pk2.Usb_read_array[3] & 0x01) > 0)
                updateInputBox(textBoxPin6In, "1");
            else
                updateInputBox(textBoxPin6In, "0");   
                 
            
        }
        
        private void updateInputBox(TextBox inputBox, string value)
        {
            if (inputBox.Enabled)
            {
                inputBox.Text = value;
                if (value == "1")
                    inputBox.BackColor = Color.DodgerBlue;
                else
                    inputBox.BackColor = Color.DarkBlue;
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(FormPICkit2.HomeDirectory + "\\Logic Tool User Guide.pdf");
            }
            catch
            {
                MessageBox.Show("Unable to open Logic Tool User Guide.");
            }
            
        }

        private void checkBoxVDD_Click(object sender, EventArgs e)
        {
            VddCallback(true, checkBoxVDD.Checked);
        }

    }
}