using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Pk3 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
    static class PK3Helpers
    {
        public static DelegateResetStatusBar ResetStatusBar;
        public static DelegateStepStatusBar StepStatusBar;    

        static public bool fwdownloadsuccess = false;
        static public byte os_typ = 0;
        static public byte os_maj = 0;
        static public byte os_min = 0;
        static public byte os_rev = 0;

        static public byte ap_typ = 0;
        static public byte ap_maj = 0;
        static public byte ap_min = 0;
        static public byte ap_rev = 0;

        static public int MagicKey = 0;
        static public bool firmwareInvalid = true;

        static private uint GetTwosCompWordChecksum(byte[] Data)
        {
            uint checksum = 0;

            // If the buffer is not word-aligned, add a Zero byte to the end
            if ((Data.Length % 2) != 0)
            {
                Array.Resize<byte>(ref Data, Data.Length + 1);
                Data[Data.Length - 1] = 0;
            }

            for (int i = 0; i < Data.Length; i += 2)
            {
                checksum += (uint)(Data[i] << 8) | (uint)Data[i + 1];
            }

            checksum = (~checksum) + 1;

            return checksum;
        }

        static private bool SendBulkData(byte[] Data)
        {
            /* From MPPICKIT3:
             *
             * This class allows the user to send/receive any amount of data as a series of
             * HID reports. It isolates the user from knowing how to hook to MPLABComm
             * and from knowing how data is turned into reports. You basically, open,
             * read, write and close. It extends HID which is the class that actually
             * knows how to send/receive reports.
             *
             * Protocol notes: HID wants to send information in reports. We chose a report size of 64 bytes.
             * The Real ICE/ICD3/PK3, do not have a symetric protocol implementation with respect to the PC. 
             * In other words, the requirements for the USB driver implementation in the embedded side of
             * these tools are not the same as the requirements of the USB driver in the PC side.
             * If you look at the PK3 FW driver you will see it does not look like the GetData/SendData defined
             * here. However, both the transmission and reception use the same protocol format.
             * In the PC side, GetData gets called and we can safely assume that GetData will not
             * need to read from the middle of a message. GetData is called to get a message. This message
             * might have the excepted len (passed as a parameter) or any other len. As long as a message
             * is received, GetData must return indicating how much data it got.
             * Of course, that means that when calling GetData, you'd better pass a buffer big enough
             * to handle the largest possible response (which might not be the one you were expecting)
             * the PK3 can send.
             * This allows the following situation to occur:
             *      PC -> Send command
             *      PK3 -> Send response
             *  Or
             *      PC -> Send command
             *      PK3 -> Send working indication
             *      PK3 -> Send response
             *
             * Note: the following examples show how data is packetized into reports. The
             * examples are the same regardless of who is sending our who is receiving
             * (PC or PK3 FW).*
             * An example of 32 (0x20) bytes of data being sent in s single report
             *
             *     +----------+
             *     | data1    | byte 0      <-- beginnig of report 1
             *     | data2    | byte 6
             *     :          :
             *     | 0x20     | byte 60     <- length of data in this message (little endian)
             *     | 0x00     | byte 61        In this case the length is 0x20 or 32 bytes
             *     | 0x00     | byte 62
             *     | 0x00     | byte 63     <-- end of report 1. End of transmission
             *     +----------+
             *
             *
             * An example of 67 bytes (0x43 in hex) being sent. It
             * requires sending 2 reports with 60 bytes in the first one (64 bytes of
             * report size - 4 bytes for the length) and 7 in the
             *
             *     +----------+
             *     | data1    | byte 0      <-- beginnig of report 1
             *     | data2    | byte 6
             *     :          :
             *     | 0x43     | byte 60     <- length of data in this message (little endian)
             *     | 0x00     | byte 61        In this case the length is 0x43 or 67 bytes
             *     | 0x00     | byte 62
             *     | 0x00     | byte 63     <-- end of report 1
             *     +----------+
             *     +----------+
             *     | data61   | byte 0      <-- beginning of report 2
             *     | data62   |
             *     | data63   |
             *     | data64   |
             *     | data65   |
             *     | data66   |
             *     :          :
             *     :          :
             *     | padding  | byte 60     <--- note NO length included here. Only in the
             *     | padding  | byte 61          very first report
             *     | padding  | byte 62
             *     | padding  | byte 63     <-- end of report 2
             *     +----------+
             *
             * @author jose
             */

             // Add the total message length at the end of the first USB report
            int size = Data.Length + 2 + 4;
            // Round the packet size up to the nearest USB report size
            if (size % 64 != 0) size = size + (64 - size % 64); 
            byte[] TotalData = new byte[size];

            // Fill the buffer with our pad value. Useful data will overwrite as necessary.
            for (int i = 0; i < TotalData.Length; i++) TotalData[i] = 0x5B;

            if (Data.Length <= 60)
            { // Only one report needed
                Array.Copy(Data, 0, TotalData, 0, Data.Length);
            }
            else
            { // Muliple reports needed
                Array.Copy(Data, 0, TotalData, 0, 60);
                Array.Copy(Data, 60, TotalData, 64, Data.Length - 60);
            }

            TotalData[60] = Convert.ToByte((Data.Length + 2) & 0xFF);
            TotalData[61] = Convert.ToByte(((Data.Length + 2) >> 8) & 0xFF);
            TotalData[62] = Convert.ToByte(((Data.Length + 2) >> 16) & 0xFF);
            TotalData[63] = Convert.ToByte(((Data.Length + 2) >> 24) & 0xFF);

            uint Checksum = GetTwosCompWordChecksum(Data);

            // If the data packet is more than one report big, add 4 to the index to skip
            // over the Bulk Transfer Checksum
            if (TotalData.Length > 0x40)
            {
                TotalData[Data.Length + 4 + 0] = Convert.ToByte(Checksum & 0xFF);
                TotalData[Data.Length + 4 + 1] = Convert.ToByte((Checksum >> 8) & 0xFF);
            }
            else
            { // The packet is only 1 report, so we don't need to adjust for the DWORD Length
                TotalData[Data.Length + 0] = Convert.ToByte(Checksum & 0xFF);
                TotalData[Data.Length + 1] = Convert.ToByte((Checksum >> 8) & 0xFF);
            }

            byte[] outgoingData = new byte[64];

            ResetStatusBar((int)(TotalData.Length / 64));
            for (int reportCount = 0; reportCount * 64 < TotalData.Length; reportCount++)
            {
                Array.Copy(TotalData, reportCount * 64, outgoingData, 0, 64);

                Pk3.writeUSB(outgoingData);

                StepStatusBar();
            }

            if(Pk3.readUSB())
            {
                // Evaluate response to determine success or failure
                if ((Pk3.Usb_read_array[1] == 0x00) && (Pk3.Usb_read_array[2] == 0x00))
                { // we got a "success" indicator
                    return true;
                }
                else
                { // we got something crazy
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        static private bool ReceiveCommandResponse(uint Command, out byte[] DataReceived)
        {   //TODO: Now that ReceiveCommandResponse() throws an exception instead of returning "false" on USB errors, make DataReceived a return value instead of an [out] parameter.
            //TODO: This method uses 3 different ways to control loops: if/else, break/continue, and straight returns inside code. Pick one and clean up.
            byte[] usbReadBuffer = new byte[64];
            byte[] usbCommand = new byte[] { (byte)(((ushort)Command) & 0xFF), (byte)(((ushort)Command >> 8) & 0xFF) };
            bool ExitLoop = false;
            uint DataLength = 0;
            
            // Temporary assignment to make the compiler happy.
            DataReceived = null;

            // Internally we will use a generic list to handle output data.
            List<byte> retval = new List<byte>();

            while (ExitLoop == false)
            {
                Pk3.readUSB(); // Remember: readUSB() is a blocking function

                // Grab the payload size from the message, subtracting the command echo and success indicator
                DataLength = checked((uint)(Pk3.Usb_read_array[61] + (Pk3.Usb_read_array[62] << 8) +
                                    (Pk3.Usb_read_array[63] << 16) + (Pk3.Usb_read_array[64] << 24) - 4));
                DataReceived = new byte[DataLength];

                // If the received report does not include the command echo, reject it. The 
                // command was not echoed properly.
                if ((Pk3.Usb_read_array[1] != usbCommand[0]) || (Pk3.Usb_read_array[2] != usbCommand[1]))
                { // This is a protocol exception.
                    return false;
                }

                // The 3rd and 4th byte are either the Success/Fail indicator bytes or the Working
                // Indicator packet. 
                if (Pk3.Usb_read_array[3] == 0x3F && Pk3.Usb_read_array[4] == 0x00)
                { // Received Working Indicator... need to try again for package.
                    while (ExitLoop == false)
                    {
                        Pk3.readUSB();

                        // Grab the payload size from the message, subtracting the success indicator
                        DataLength = checked((uint)(Pk3.Usb_read_array[61] + (Pk3.Usb_read_array[62] << 8) +
                                            (Pk3.Usb_read_array[63] << 16) + (Pk3.Usb_read_array[64] << 24) - 4));
                        DataReceived = new byte[DataLength];

                        // In subsequent receptions, there is no Command Echo, so everything 
                        // is shifted to the left by two bytes. The Success or Working Indicator
                        // will appear at bytes 0 and 1.

                        if ((Pk3.Usb_read_array[1] == 0x3F) && (Pk3.Usb_read_array[2] == 0x00))
                        { // We received a Working Indicator. Try again with the next report.
                            continue;
                        }
                        else if ((Pk3.Usb_read_array[1] == 0x00) && (Pk3.Usb_read_array[2] == 0x00))
                        { // We received a Success Indicator. Declare success.
                            if (DataLength > 0)
                            { // Length is non-zero, then the returning data needs to be passed to the calling function
                                Array.Copy(Pk3.Usb_read_array, 3, DataReceived, 0, DataLength);
                            }
                            ExitLoop = true;
                            break;
                        }
                        else if ((Pk3.Usb_read_array[1] == 0xFF) && (Pk3.Usb_read_array[2] == 0x00))
                        {   // We received a Fail Indicator. Declare failure.
                            // NOTE FOR DEBUGGERS: We expect VS's debugger to stop here, but this
                            // actually gets handled in the BackgroundWorker's RunWorkCompleted 
                            // callback. Just hit "play" to continue debugging.
                            return false;
                        }
                        else
                        { // Any other received message is an error. 
                            //Debugger.Break();
                            return false;
                        }
                    }
                    continue;
                }
                else if ((Pk3.Usb_read_array[3] == 0x00) && (Pk3.Usb_read_array[4] == 0x00))
                {   // Received "Success" indicator. Copy any data values over and return (ignore command echo and success indicator)
                    // If the incoming data is longer than a USB report size, we will have to grab more than one.
                    // See SendReadMemObj() method. It is doing basically the same thing.
                    if (DataLength == 0)
                    {
                        
                    }
                    else if (DataLength <= 60)
                    {   // Incoming data fits in one USB report. Remember: the length includes 
                        // the command echo and success/fail indicator. We only want to return 
                        // the actual data, so we can ignore those parts. We have to subtract
                        // this size from the total size for the array copy, or else we will
                        // include the Length data as part of the return data.
                        Array.Copy(Pk3.Usb_read_array, 5, DataReceived, 0, DataLength);
                    }
                    else
                    {   // Incoming data requires more than one report.
                        // Grab the first report. Its size is different than the rest because of
                        // the Command Echo and Success/Fail indicator. (56 bytes of data)
                        Array.Copy(Pk3.Usb_read_array, 5, Pk3.Usb_read_array, 0, 56);
                        Array.Resize<byte>(ref Pk3.Usb_read_array, 56); // Resize, so we can use .AddRange() to add the data below.
                        retval.AddRange(Pk3.Usb_read_array); // We'll use the generic list for this part

                        // Tracks the expected number of data bytes in the next USB report
                        uint IncomingDataLength = 56;

                        // Get the rest of the data. Note that incrementing the loop control variable is
                        // done within the loop
                        for (uint DataLeftToGet = DataLength - 56; DataLeftToGet > 0; DataLeftToGet -= IncomingDataLength)
                        {
                            if (DataLeftToGet > 64) IncomingDataLength = 64;
                            else IncomingDataLength = DataLeftToGet;

                            Pk3.readUSB();

                            // Adjust the array in case we're receiving less than the USB report size of data
                            if (IncomingDataLength < 64)
                            {
                                byte[] smallerArray = new byte[IncomingDataLength];
                                Array.Copy(Pk3.Usb_read_array, 1, smallerArray, 0, smallerArray.Length);
                                retval.AddRange(smallerArray);
                            }
                            else
                            {
                                Array.Copy(Pk3.Usb_read_array, 1, Pk3.Usb_read_array, 0, 64);
                                retval.AddRange(Pk3.Usb_read_array);
                            }
                        }

                        DataReceived = retval.ToArray(); 
                    }
                    ExitLoop = true;
                    continue;
                }
                else if ((Pk3.Usb_read_array[3] == 0xFF) && (Pk3.Usb_read_array[4] == 0x00))
                { // Received "Fail" indicator.
                    return false;
                }
                else
                {   // If any other value is found, assume failure.
                    //Debugger.Break();
                    return false;
                }
            }

            return true;
        }

        static private void SendCommandWithDataNoResponse(uint Command, byte[] Data)
        {
            /* HID report payloads are 64 bytes. The PICkit3 message structure requires the first 
             * report have the 2-byte command and 4-byte message size. The rest of that report can
             * be payload for the command. Subsequent reports in the same message do not require 
             * the command or the size. Message size is Data Bytes + Command Bytes. 
             */

            byte[] usbCommand = new byte[] { (byte)(((ushort)Command) & 0xFF), (byte)(((ushort)Command >> 8) & 0xFF) };
            //byte[] usbWriteBuffer = new byte[64];
            int DataLength = 0;

            if (Data == null) DataLength = 0; else DataLength = Data.Length;
            
            //// WARNING: This assumes ONLY ONE report is needed (So don't do memory transfers yet!).
            //if ((Data != null) && (Data.Length > 58)) // 64 - 4 (length) - 2 (command)
            //    throw new ArgumentException("Data transfer must require only one USB transaction.", "Data");

            // Add the total message length at the end of the first USB report
            int size = DataLength + 2 + 4;
            // Round the packet size up to the nearest USB report size
            if (size % 64 != 0) size = size + (64 - size % 64);
            byte[] usbWriteBuffer = new byte[size];

            // Fill the buffer with our pad value. Useful data will overwrite as necessary.
            for (int i = 0; i < usbWriteBuffer.Length; i++) usbWriteBuffer[i] = 0x5B;

            // Copy the command into the buffer.
            usbWriteBuffer[0] = (byte)(((ushort)Command) & 0xFF);
            usbWriteBuffer[1] = (byte)(((ushort)Command >> 8) & 0xFF);

            // Copy the data into the buffer.
            if (Data == null)
            { // No data to send. Only one report needed.
            }
            else if (Data.Length <= 58) // 64 - 4 (length) - 2 (command)
            { // Only one report needed
                Array.Copy(Data, 0, usbWriteBuffer, 2, Data.Length);
            }
            else
            { // Muliple reports needed
                Array.Copy(Data, 0, usbWriteBuffer, 2, 60);
                Array.Copy(Data, 58, usbWriteBuffer, 64, Data.Length - 58);
            }

            // Fill in the length of the message
            usbWriteBuffer[60] = Convert.ToByte(DataLength + usbCommand.Length & 0xFF);
            usbWriteBuffer[61] = Convert.ToByte(DataLength + usbCommand.Length >> 8 & 0xFF);
            usbWriteBuffer[62] = Convert.ToByte(DataLength + usbCommand.Length >> 16 & 0xFF);
            usbWriteBuffer[63] = Convert.ToByte(DataLength + usbCommand.Length >> 24 & 0xFF);

            // Note that a checksum is not sent in this type of transaction.

            byte[] outgoingData = new byte[64];

            for (int reportCount = 0; reportCount * 64 < usbWriteBuffer.Length; reportCount++)
            {
                Array.Copy(usbWriteBuffer, reportCount * 64, outgoingData, 0, 64);

                Pk3.writeUSB(outgoingData);
            }
        }

        static public byte[] SendCommandWithDataAndResponse(uint Command, byte[] Data)
        {
            byte[] usbReadBuffer = new byte[64];

            SendCommandWithDataNoResponse(Command, Data);

            // Read the PK3 response and look for the command echo.
            ReceiveCommandResponse(Command, out usbReadBuffer);

            return usbReadBuffer;
        }

        static private void SendCommandWithData(uint Command, byte[] Data)
        {
            SendCommandWithDataAndResponse(Command, Data);
            return;
        }

        static public Dictionary<uint, byte> ImportHexFile(string filePath)
        {  // NOTE: The device buffers being read into must all be set to blank value before getting here!

            Dictionary<uint, byte> returnData = new Dictionary<uint, byte>();

            FileInfo hexFile = new FileInfo(filePath);
            TextReader hexRead = hexFile.OpenText();
            int segmentAddress = 0;
            string fileLine = "";

            while ((fileLine = hexRead.ReadLine()) != null)
            {
                if (fileLine[0] != ':') continue;   // skip line if not hex line entry
                if (fileLine.Length < 11) // ... or not minimum length ":BBAAAATTCC"
                {
                    continue;
                }

                int byteCount = Int32.Parse(fileLine.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                int fileAddress = segmentAddress + Int32.Parse(fileLine.Substring(3, 4), System.Globalization.NumberStyles.HexNumber);
                int recordType = Int32.Parse(fileLine.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);

                if (recordType == 0)
                { // Data Record
                    if (fileLine.Length < (11 + (2 * byteCount))) // skip if line isn't long enough for bytecount. 
                    {
                        continue;
                    }

                    for (int lineByte = 0; lineByte < byteCount; lineByte++)
                    {
                        int byteAddress = fileAddress + lineByte;
                        // get the byte value from hex file
                        string dataString = fileLine.Substring((9 + (2 * lineByte)), 2);
                        byte dataByte = Convert.ToByte(dataString, 16);
                        returnData.Add((uint)byteAddress, dataByte);
                    }
                } // end if (recordType == 0)  

                if ((recordType == 2) || (recordType == 4))
                { // Segment address
                    if (fileLine.Length != (11 + (2 * byteCount)))
                    {   // The line is the incorrect length.                                              
                        // TODO return error
                    }

                    segmentAddress = Int32.Parse(fileLine.Substring(9, 4), System.Globalization.NumberStyles.HexNumber);

                    if (recordType == 2)
                    {
                        segmentAddress <<= 4;
                    }
                    else
                    {
                        segmentAddress <<= 16;
                    }

                } // end if ((recordType == 2) || (recordType == 4)) 

                if (recordType == 1)
                { // end of record
                    break;
                }
            }

            hexRead.Close();

            return returnData;
        }

        static public int ImportHexFileAsBytes(out byte[] Buf, string filePath, out uint StartAddr)
        {
            Dictionary<uint, byte> data = ImportHexFile(filePath);

            StartAddr = uint.MaxValue;
            uint LastAddress = 0;
            int ByteCount = 0;

            // Go through the collection once to determine the size of array needed to hold the data
            foreach (KeyValuePair<uint, byte> kvp in data)
            {
                if (kvp.Key < StartAddr) StartAddr = kvp.Key;
                else if (kvp.Key > LastAddress) LastAddress = kvp.Key;
            }

            ByteCount = (int)(LastAddress - StartAddr + 1);

            //List<byte> returnData = new List<byte>(LastAddress - FirstAddress);
            Buf = new byte[ByteCount];

            // Set the "blank" value to be all 1s
            for (int i = 0; i < Buf.Length; i++)
            {
                Buf[i] = byte.MaxValue;
            }

            // Go through the collection and stuff the array with data
            foreach (KeyValuePair<uint, byte> kvp in data)
            {
                Buf[kvp.Key - StartAddr] = kvp.Value;
            }

            return ByteCount;
        }

        static private int AdjustHexFileDataForSend(ref byte[] Buf)
        {
            // the +3 makes newSize be a multiple of 4 before we multiply by 3
            // we should always get a NumBytes which is a multiple of 4 but this
            // is just in case (for testing this function NumBytes might not be
            // a multiple of 4.
            int NewLength = ((Buf.Length + 3) / 4) * 3;

            // Now make sure the new buffer is a multiple of 192 (since that is the row size for PIC24F)
            NewLength = ((NewLength + 191) / 192) * 192;

            byte[] NewBuf = new byte[NewLength];

            for (int i = 0, j = 0; i < Buf.Length; i += 4, j += 3)
            {
                NewBuf[j] = Buf[i];
                NewBuf[j + 1] = Buf[i + 1];
                NewBuf[j + 2] = Buf[i + 2];
            }

            Buf = NewBuf;

            return NewLength;
        }

        static public KONST.PICkit2USB WriteProgrammerOs(string ApHexFile, uint Command)
        {
            //TODO There is a "Magic Key" in APs and RS that we should check.
            uint StartAddr = 0;
            int NumBytes = 0;
            byte[] Ap;
            //uint connRetries = 0;

            NumBytes = ImportHexFileAsBytes(out Ap, ApHexFile, out StartAddr);
            NumBytes = AdjustHexFileDataForSend(ref Ap);

            // Adjust the start address for the programmer
            StartAddr = StartAddr / 2; // dsPIC has 2 bytes per program word

            byte[] memObj = new byte[8];

            // MemObj start address
            memObj[0] = Convert.ToByte(StartAddr & 0xFF);
            memObj[1] = Convert.ToByte((StartAddr >> 8) & 0xFF);
            memObj[2] = Convert.ToByte((StartAddr >> 16) & 0xFF);
            memObj[3] = Convert.ToByte((StartAddr >> 24) & 0xFF);

            // MemObj length in bytes
            uint RangeInBytes = (uint)Ap.Length;
            memObj[4] = Convert.ToByte(RangeInBytes & 0xFF);
            memObj[5] = Convert.ToByte((RangeInBytes >> 8) & 0xFF);
            memObj[6] = Convert.ToByte((RangeInBytes >> 16) & 0xFF);
            memObj[7] = Convert.ToByte((RangeInBytes >> 24) & 0xFF);

            SendCommandWithData(Command, memObj);
            if (Command == 0x23)
                Thread.Sleep(3000); // Wait for bootloader
            SendBulkData(Ap);

            // The programmer will restart now, so we need to reinit comms
            Thread.Sleep(2000);

            KONST.PICkit2USB res = KONST.PICkit2USB.notFound;

            // while (res == KONST.PICkit2USB.notFound || ((res == KONST.PICkit2USB.readError || res == KONST.PICkit2USB.readwriteError || res == KONST.PICkit2USB.writeError) && connRetries < 10))
            while (res == KONST.PICkit2USB.notFound)
               {
                res = Pk3.DetectPICkit2Device(FormPICkit2.pk2number, true);
                Thread.Sleep(500); // Delay a bit so we don't hammer the programmer
                // connRetries++;
            }

            //Initialize();

            return res;
        }
    }
}
