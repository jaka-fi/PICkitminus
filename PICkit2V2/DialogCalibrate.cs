using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Pk2 = PICkit2V2.PICkitFunctions;
using KONST = PICkit2V2.Constants;

namespace PICkit2V2
{
	public partial class DialogCalibrate : Form
	{
		private bool unitIDChanged = false;

		public DialogCalibrate()
		{
			InitializeComponent();
			Pk2.VddOff();
			Pk2.ForcePICkitPowered();
			setupClearButtons();

			if (Pk2.isPK3)
			{
				panelIntro.Visible = false;
				panelSetup.Visible = false;
				panelCal.Visible = false;
				panelUnitID.Visible = true;
				textBoxUnitID.Text = Pk2.UnitIDRead();
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Pk2.VddOff();
			if (unitIDChanged)
			{
				// reset PICkit so it re-enumerates with new ID.
				Pk2.ResetPICkit2();
				Thread.Sleep(1000);

				if (Pk2.isPK3) // PICkit 3
				{
					MessageBox.Show("Resetting PICkit 3.\n\nPlease wait for USB enumeration\nto complete before clicking OK...", "Reset PICkit 3");
				}
				else // PICkit 2
				{
					MessageBox.Show("Resetting PICkit 2.\n\nPlease wait for USB enumeration\nto complete before clicking OK...", "Reset PICkit 2");
				}
				Thread.Sleep(1000);
			}
			this.Close();
		}

		private void setupClearButtons()
		{
			if (Pk2.isPK3) // PICkit 3
			{
				buttonBack.Enabled = false;
				buttonBack.Visible = false;
				buttonNext.Enabled = false;
				buttonNext.Visible = false;

				if (Pk2.UnitIDRead().Length > 0)
				{
					buttonClearUnitID_PK3.Enabled = true;
					buttonClearUnitID_PK3.Visible = true;
					buttonSetUnitID.Enabled = false;
					buttonSetUnitID.Visible = false;
				}
				else
				{
					buttonClearUnitID_PK3.Enabled = false;
					buttonClearUnitID_PK3.Visible = false;
					buttonSetUnitID.Enabled = true;
					buttonSetUnitID.Visible = true;
				}
			}
			else // PICkit 2
			{
				buttonClearUnitID_PK3.Enabled = false;
				buttonClearUnitID_PK3.Visible = false;
				buttonClearUnitID_PK3.Text = "";

				if (Pk2.isCalibrated())
				{
					buttonClearCal.Enabled = true;
					buttonClearCal.Text = "Clear Calibration";
				}
				else
				{
					buttonClearCal.Enabled = false;
					buttonClearCal.Text = "Unit Not Calibrated";
				}

				if (Pk2.UnitIDRead().Length > 0)
				{
					buttonClearUnitID.Enabled = true;
					buttonClearUnitID.Text = "Clear Unit ID";
				}
				else
				{
					buttonClearUnitID.Enabled = false;
					buttonClearUnitID.Text = "No Assigned ID";
				}
			}
		}

		private void buttonNext_Click(object sender, EventArgs e)
		{
			if (panelIntro.Visible)
			{
				panelIntro.Visible = false;
				panelSetup.Visible = true;
				buttonBack.Enabled = true;
			}
			else if (panelSetup.Visible)
			{
				panelSetup.Visible = false;
				panelCal.Visible = true;
				buttonCalibrate.Enabled = true;
				labelGoodCal.Visible = false;
				labelBadCal.Visible = false;
				textBoxVDD.Text = string.Format("{0:0.000}", 4F);
				textBoxVDD.Focus();
				textBoxVDD.SelectAll();
				Pk2.SetVoltageCals(0x0100, 0x00, 0x80); // set to defaults.
				Pk2.SetVDDVoltage(4.0F, 3.4F, false);
				Pk2.VddOn();
			}
			else if (panelCal.Visible)
			{
				panelCal.Visible = false;
				panelUnitID.Visible = true;
				buttonSetUnitID.Enabled = true;
				labelAssignedID.Visible = false;
				textBoxUnitID.Text = Pk2.UnitIDRead();
				textBoxUnitID.Focus();
				textBoxVDD.SelectAll();
				buttonNext.Enabled = false;
				buttonCancel.Text = "Finished";
				Pk2.VddOff();
			}
		}

		private void buttonBack_Click(object sender, EventArgs e)
		{
			if (panelSetup.Visible)
			{
				panelIntro.Visible = true;
				panelSetup.Visible = false;
				buttonBack.Enabled = false;
				setupClearButtons();
			}
			else if (panelCal.Visible)
			{
				Pk2.VddOff();
				panelSetup.Visible = true;
				panelCal.Visible = false;

			}
			else if (panelUnitID.Visible)
			{
				panelUnitID.Visible = false;
				panelCal.Visible = true;
				buttonCalibrate.Enabled = false;
				labelGoodCal.Visible = false;
				labelBadCal.Visible = false;
				textBoxVDD.Text = "-";
				buttonNext.Enabled = true;
				buttonCancel.Text = "Cancel";
			}
		}

		private void buttonCalibrate_Click(object sender, EventArgs e)
		{
			float Vdd = 0, Vpp = 0;
			float measuredVdd = 0;
			bool calSucceed = true;

			try
			{
				measuredVdd = float.Parse(textBoxVDD.Text);
			}
			catch
			{
				MessageBox.Show("Invalid 'volts measured' value.");
				return;
			}

			// Cal the ADC results
			Pk2.ReadPICkitVoltages(ref Vdd, ref Vpp);
			measuredVdd /= Vdd;  //ratio
			if (measuredVdd > 1.25F)
			{
				measuredVdd = 1.25F;
				calSucceed = false;
			}
			if (measuredVdd < 0.75F)
			{
				measuredVdd = 0.75F;
				calSucceed = false;
			}
			float calFactor = 256F * measuredVdd; // 512 is 1:1 calibration factor
			Pk2.SetVoltageCals((ushort)calFactor, 0x00, 0x80);  // leave Vdd cals unchanged for now.

			// Now that we have an accurate ADC reading, we can self-cal the VDD setpoints.
			// Vdd Offset = (3 - (4*V3)/V4)*CCP4
			// Vdd CalFactor = 1/(V4 - V3) * 128.
			//     where V3 = actual voltage at SetVDDVoltage(3.0)
			//           V4 = actual voltage at SetVDDVoltage(4.0)
			//           CCP4 = CalculateVddCPP(4.0F) >> 6
			float offset = 0;
			float calCCP = 0;
			float Vdd3v = 0;
			Pk2.SetVDDVoltage(3.0F, 2.00F, false);
			Thread.Sleep(150);
			Pk2.ReadPICkitVoltages(ref Vdd, ref Vpp);
			Vdd3v = Vdd;
			Pk2.SetVDDVoltage(4.0F, 2.70F, false);
			Thread.Sleep(150);
			Pk2.ReadPICkitVoltages(ref Vdd, ref Vpp);
			offset = (3 - (4 * Vdd3v) / Vdd) * (Pk2.CalculateVddCPP(4.0F) >> 6);
			if (offset > 127F)
			{
				offset = 127F;
				calSucceed = false;
			}
			if (offset < -128F)
			{
				offset = -128F;
				calSucceed = false;
			}
			calCCP = (1 / (Vdd - Vdd3v)) * 128;
			if (calCCP > 173) // 135%
			{
				calCCP = 173;
				calSucceed = false;
			}
			if (calCCP < 83) // 65%
			{
				calCCP = 83;
				calSucceed = false;
			}

			if (calSucceed)
			{
				labelGoodCal.Visible = true;
				labelBadCal.Visible = false;
				Pk2.SetVoltageCals((ushort)calFactor, (byte)offset, (byte)(calCCP + 0.5));
			}
			else
			{
				labelGoodCal.Visible = false;
				labelBadCal.Visible = true;
				Pk2.SetVoltageCals(0x0100, 0x00, 0x80); // leave uncal'd
			}
			buttonCalibrate.Enabled = false;
			Pk2.VddOff();
		}

		private void textBoxUnitID_TextChanged(object sender, EventArgs e)
		{
			if (textBoxUnitID.Text.Length > 14)
			{
				textBoxUnitID.Text = textBoxUnitID.Text.Substring(0, 14);
				textBoxUnitID.SelectionStart = 14;
			}
		}

		private void buttonSetUnitID_Click(object sender, EventArgs e)
		{
			if (Pk2.UnitIDWrite(textBoxUnitID.Text))
			{
				if (Pk2.isPK3) // PICkit 3
				{
					labelAssignedID.Text = "Unit ID Assigned to this PICkit 3.";
					buttonCancel.Text = "Finished";
					textBoxUnitID.Enabled = false;
				}

				labelAssignedID.Visible = true;
				buttonSetUnitID.Enabled = false;
				unitIDChanged = true;
			}
		}

		private void buttonClearCal_Click(object sender, EventArgs e)
		{
			Pk2.SetVoltageCals(0x0100, 0x00, 0x80); // leave uncal'd
			buttonClearCal.Enabled = false;
			buttonClearCal.Text = "Unit Not Calibrated";
		}

		private void buttonClearUnitID_Click(object sender, EventArgs e)
		{
			Pk2.UnitIDWrite("");
			buttonClearUnitID.Enabled = false;
			buttonClearUnitID.Text = "No Assigned ID";
			unitIDChanged = true;
		}

		private void buttonClearUnitID_PK3_Click(object sender, EventArgs e)
		{
			Pk2.UnitIDWrite("");
			labelAssignedID.Text = "No Unit ID Assigned to this PICkit 3.";
			labelAssignedID.Visible = true;
			buttonClearUnitID_PK3.Enabled = false;
			buttonClearUnitID_PK3.Text = "No Assigned ID";
			buttonCancel.Text = "Finished";
			textBoxUnitID.Text = "";
			textBoxUnitID.Enabled = false;
			unitIDChanged = true;
		}

		private void DialogCalibrate_Shown(object sender, EventArgs e)
		{
			if (Pk2.isPK3) // PICkit 3
			{
				textBoxUnitID.Focus();
				textBoxUnitID.SelectAll();
			}
		}
	}
}