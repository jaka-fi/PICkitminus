Release Notes for PICkit(R) 2 Microcontroller Programmer
PICkit 2    V2.61.00
Device File V1.61.00

*** Important
*** PICkit 2 software v2.61.00 requires PICkit 2 OS firmware
*** Version 2.32.00 before the application will work correctly.
*** Use the "Download PICkit 2 Firmware" selection on the 
*** Tools dropdown menu.  The new OS is normally located at
*** C:\Program Files\Microchip\PICkit 2 v2\PK2V023200.hex

24 March 2009
-----------------------------------------------------------------
Table of Contents
-----------------------------------------------------------------
1. Device Support List
2. Operating System Support List
3. Release notes

-----------------------------------------------------------------
1. Device Support List
-----------------------------------------------------------------

=================================================================
= NOTE: This list shows support for the PICkit 2 Programmer     =
= software application.  It does not show support for using the =
= PICkit 2 within MPLAB IDE.  For a list of MPLAB supported     =
= parts, see the MPLAB IDE PICkit 2 Readme.                     =
= (Typically in C:\Program Files\Microchip\MPLAB IDE\Readmes)   =
=================================================================


* Indicates new parts supported in this release with v1.61 of the
  device file.

+ Indicates parts that require 4.75V minimum VDD for programming.
  PICkit 2 may not be able to generate sufficiently high VDD,
  so an external 5.0v power supply may be required.

# indicates Midrange parts that support low Vdd programming


Baseline Devices
----------------
PIC10F200       PIC10F202       PIC10F204       PIC10F206
PIC10F220       PIC10F222
PIC12F508       PIC12F509       PIC12F510	PIC12F519 
PIC16F505       PIC16F506       PIC16F526 
PIC16F54        PIC16F57        PIC16F59


Midrange/Standard Devices
----------------
>> All 'LF' versions of devices are supported
PIC12F609       PIC12HV609 	
PIC12F615       PIC12HV615      
PIC12F629       PIC12F635#      PIC12F675       PIC12F683#
PIC16F610       PIC16HV610      PIC16F616       PIC16HV616
PIC16F627       PIC16F628       PIC16F639 
PIC16F627A      PIC16F628A      PIC16F648A
PIC16F630       PIC16F631       PIC16F636#      PIC16F676
PIC16F677       PIC16F684#      PIC16F685#      PIC16F687#
PIC16F688#      PIC16F689#      PIC16F690#      
PIC16F72+
PIC16F73+       PIC16F74+       PIC16F76+       PIC16F77+
PIC16F716 
PIC16F737+      PIC16F747+      PIC16F767+      PIC16F777+
PIC16F785       PIC16HV785      
PIC16F84A       PIC16F87#       PIC16F88#
PIC16F818#      PIC16F819# 
PIC16F870       PIC16F871       PIC16F872       
PIC16F873       PIC16F874       PIC16F876       PIC16F877 
PIC16F873A      PIC16F874A      PIC16F876A      PIC16F877A
PIC16F882#
PIC16F883#      PIC16F884#      PIC16F886#      PIC16F887#
PIC16F913#      PIC16F914#      PIC16F916#      PIC16F917#
PIC16F946#

Midrange/1.8V Min Devices
----------------
PIC16F722       PIC16LF722 
PIC16F723       PIC16LF723      PIC16F724       PIC16LF724 
PIC16F726       PIC16LF726      PIC16F727       PIC16LF727 

PIC16F1933      PIC16F1934      PIC16F1936      PIC16F1937 
PIC16F1938      PIC16F1939 
PIC16LF1933     PIC16LF1934     PIC16LF1936     PIC16LF1937 
PIC16LF1938     PIC16LF1939 


PIC18F Devices
--------------
>> All 'LF' versions of devices are supported
PIC18F242       PIC18F252       PIC18F442       PIC18F452
PIC18F248       PIC18F258       PIC18F448       PIC18F458
PIC18F1220      PIC18F1320      PIC18F2220  
PIC18F1230      PIC18F1330      PIC18F1330-ICD    
PIC18F2221      PIC18F2320      PIC18F2321      PIC18F2331      
PIC18F2410      PIC18F2420      PIC18F2423      PIC18F2431
PIC18F2450      PIC18F2455      PIC18F2458      PIC18F2480
PIC18F2510      PIC18F2515      PIC18F2520      PIC18F2523  
PIC18F2525      PIC18F2550      PIC18F2553      PIC18F2580
PIC18F2585	
PIC18F2610      PIC18F2620      PIC18F2680      PIC18F2682  
PIC18F2685 
PIC18F4220      PIC18F4221      PIC18F4320      PIC18F4321  
PIC18F4331      PIC18F4410      PIC18F4420      PIC18F4423      
PIC18F4431      PIC18F4450      PIC18F4455      PIC18F4458
PIC18F4480      
PIC18F4510      PIC18F4515      PIC18F4520      PIC18F4523 
PIC18F4525      PIC18F4550      PIC18F4553      PIC18F4580
PIC18F4585
PIC18F4610      PIC18F4620      PIC18F4680      PIC18F4682  
PIC18F4685      PIC18F6310      PIC18F6390      PIC18F6393 
PIC18F6410      PIC18F6490      PIC18F6493      PIC18F6520
PIC18F6525      PIC18F6527      
PIC18F6585      PIC18F6620      PIC18F6621      PIC18F6622
PIC18F6627      PIC18F6628      PIC18F6680      PIC18F6720
PIC18F6722      PIC18F6723 
PIC18F8310      PIC18F8390      PIC18F8393      PIC18F8410
PIC18F8490      PIC18F8493 
PIC18F8520      PIC18F8525      PIC18F8527      PIC18F8585 
PIC18F8620      PIC18F8621      PIC18F8622      PIC18F8627
PIC18F8628
PIC18F8680      PIC18F8720      PIC18F8722	PIC18F8723


PIC18F_J_ Devices
-----------------
PIC18F24J10     PIC18LF24J10     
PIC18F24J11     PIC18LF24J11    PIC18F24J50     PIC18LF24J50 
PIC18F25J10     PIC18LF25J10        
PIC18F25J11     PIC18LF25J11    PIC18F25J50     PIC18LF25J50 
PIC18F26J11     PIC18LF26J11    PIC18F26J50     PIC18LF26J50 
PIC18F44J10     PIC18LF44J10
PIC18F44J11     PIC18LF44J11    PIC18F44J50     PIC18LF44J50 
PIC18F45J10     PIC18LF45J10
PIC18F45J11     PIC18LF45J11    PIC18F45J50     PIC18LF45J50 
PIC18F46J11     PIC18LF46J11    PIC18F46J50     PIC18LF46J50 
PIC18F63J11     PIC18F63J90     PIC18F64J11     PIC18F64J90
PIC18F65J10     PIC18F65J11     PIC18F65J15     PIC18F65J50
PIC18F65J90  
PIC18F66J10     PIC18F66J11     PIC18F66J15     PIC18F66J16 
PIC18F66J50     PIC18F66J55     PIC18F66J60     PIC18F66J65    
PIC18F66J90 
PIC18F67J10     PIC18F67J11     PIC18F67J50     PIC18F67J60 
PIC18F67J90 
PIC18F83J11     PIC18F83J90     PIC18F84J11     PIC18F84J90 
PIC18F85J10     PIC18F85J11     PIC18F85J15     PIC18F85J50
PIC18F85J90 
PIC18F86J10     PIC18F86J11     PIC18F86J15     PIC18F86J16 
PIC18F86J50     PIC18F86J55     PIC18F86J60     PIC18F86J65 
PIC18F86J90     
PIC18F87J10     PIC18F87J11     PIC18F87J50     PIC18F87J60
PIC18F87J90 
PIC18F96J60     PIC18F96J65     PIC18F97J60 


PIC18F_K_ Devices
-----------------
PIC18F13K22     PIC18LF13K22    PIC18F14K22     PIC18LF14K22 
PIC18F13K50     PIC18LF13K50    PIC18F14K50     PIC18LF14K50 
PIC18F14K50-ICD 
PIC18F23K20     PIC18F24K20     PIC18F25K20     PIC18F26K20 
PIC18F43K20     PIC18F44K20     PIC18F45K20     PIC18F46K20  


PIC24 Devices
-------------
PIC24F04KA200   PIC24F04KA201 
PIC24F08KA101   PIC24F08KA102 
PIC24F16KA101   PIC24F16KA102 
   NOTE: To program PIC24F-KA- devices with MCLR used as IO,
         Tools > Use High Voltage Program Entry must be enabled.

PIC24FJ16GA002       PIC24FJ16GA004 
PIC24FJ32GA002       PIC24FJ32GA004 
PIC24FJ32GA102       PIC24FJ32GA104 
PIC24FJ48GA002       PIC24FJ48GA004 
PIC24FJ64GA002       PIC24FJ64GA004 
PIC24FJ64GA102       PIC24FJ64GA104 

PIC24FJ64GA006       PIC24FJ64GA008       PIC24FJ64GA010 
PIC24FJ96GA006       PIC24FJ96GA008       PIC24FJ96GA010 
PIC24FJ128GA006      PIC24FJ128GA008      PIC24FJ128GA010 
PIC24FJ128GA106      PIC24FJ128GA108      PIC24FJ128GA110 
PIC24FJ192GA106      PIC24FJ192GA108      PIC24FJ192GA110 
PIC24FJ256GA106      PIC24FJ256GA108      PIC24FJ256GA110 

PIC24FJ32GB002       PIC24FJ32GB004 
PIC24FJ64GB002       PIC24FJ64GB004 

PIC24FJ64GB106       PIC24FJ64GB108       PIC24FJ64GB110 
PIC24FJ128GB106      PIC24FJ128GB108      PIC24FJ128GB110 
PIC24FJ192GB106      PIC24FJ192GB108      PIC24FJ192GB110 
PIC24FJ256GB106      PIC24FJ256GB108      PIC24FJ256GB110 

PIC24HJ12GP201       PIC24HJ12GP202 
PIC24HJ16GP304      
PIC24HJ32GP202       PIC24HJ32GP204 
PIC24HJ32GP302       PIC24HJ32GP304
PIC24HJ64GP202       PIC24HJ64GP204
PIC24HJ64GP206       PIC24HJ64GP210       
PIC24HJ64GP502
PIC24HJ64GP504       PIC24HJ64GP506       PIC24HJ64GP510 
PIC24HJ128GP202      PIC24HJ128GP204
PIC24HJ128GP206      PIC24HJ128GP210      
PIC24HJ128GP306      PIC24HJ128GP310      
PIC24HJ128GP502      PIC24HJ128GP504
PIC24HJ128GP506      PIC24HJ128GP510 
PIC24HJ256GP206      PIC24HJ256GP210      PIC24HJ256GP610 


dsPIC33 Devices
---------------
dsPIC33FJ06GS101     dsPIC33FJ06GS102     dsPIC33FJ06GS202 
dsPIC33FJ16GS402     dsPIC33FJ16GS404 
dsPIC33FJ16GS502     dsPIC33FJ16GS504 

dsPIC33FJ12GP201     dsPIC33FJ12GP202 
dsPIC33FJ16GP304    
dsPIC33FJ32GP202     dsPIC33FJ32GP204 
dsPIC33FJ32GP302     dsPIC33FJ32GP304 
dsPIC33FJ64GP202     dsPIC33FJ64GP204 
dsPIC33FJ64GP206     dsPIC33FJ64GP306     dsPIC33FJ64GP310 
dsPIC33FJ64GP706     dsPIC33FJ64GP708     dsPIC33FJ64GP710 
dsPIC33FJ64GP802     dsPIC33FJ64GP804 
dsPIC33FJ128GP202    dsPIC33FJ128GP204 
dsPIC33FJ128GP206    dsPIC33FJ128GP306    dsPIC33FJ128GP310 
dsPIC33FJ128GP706    dsPIC33FJ128GP708    dsPIC33FJ128GP710 
dsPIC33FJ256GP506    dsPIC33FJ256GP510    dsPIC33FJ256GP710 
dsPIC33FJ128GP802    dsPIC33FJ128GP804 

dsPIC33FJ12MC201     dsPIC33FJ12MC202 
dsPIC33FJ16MC304    
dsPIC33FJ32MC202     dsPIC33FJ32MC204 
dsPIC33FJ32MC302     dsPIC33FJ32MC304 
dsPIC33FJ64MC202     dsPIC33FJ64MC204 
dsPIC33FJ64MC506     dsPIC33FJ64MC508     dsPIC33FJ64MC510 
dsPIC33FJ64MC706     dsPIC33FJ64MC710    
dsPIC33FJ64MC802     dsPIC33FJ64MC804 
dsPIC33FJ128MC202    dsPIC33FJ128MC204  
dsPIC33FJ128MC506    dsPIC33FJ128MC510    dsPIC33FJ128MC706 
dsPIC33FJ128MC708    dsPIC33FJ128MC710 
dsPIC33FJ256MC510    dsPIC33FJ256MC710 
dsPIC33FJ128MC802    dsPIC33FJ128MC804 


dsPIC30 Devices
---------------
dsPIC30F2010         dsPIC30F2011         dsPIC30F2012
dsPIC30F3010         dsPIC30F3011         dsPIC30F3012
dsPIC30F3013         dsPIC30F3014 
dsPIC30F4011         dsPIC30F4012         dsPIC30F4013
dsPIC30F5011^        dsPIC30F5013^        dsPIC30F5015
dsPIC30F5016 
dsPIC30F6010A        dsPIC30F6011A        dsPIC30F6012A
dsPIC30F6013A        dsPIC30F6014A        dsPIC30F6015

^ These 2 devices are not supported for low VDD programming.


dsPIC30 SMPS Devices
--------------------
dsPIC30F1010
dsPIC30F2020         dsPIC30F2023


PIC32 Devices
--------------------
PIC32MX320F032H      PIC32MX320F064H      PIC32MX320F128L 
PIC32MX320F128H 
PIC32MX340F128H      PIC32MX340F128L
PIC32MX340F256H 
PIC32MX340F512H*
PIC32MX360F256L      PIC32MX360F512L 
PIC32MX420F032H 
PIC32MX440F128L      PIC32MX440F128H 
PIC32MX440F256H      PIC32MX440F512H 
PIC32MX460F256L      PIC32MX460F512L 




KEELOQ HCS Devices
------------------
HCS200     HCS201     HCS300     HCS301      HCS320 
HCS360     HCS361     HCS362 

	HCSxxx File -> Import HEx Notes:
		The first line only may be imported from SQTP
                *.NUM files generated by the KEELOQ tool in 
                MPLAB.

        Connections for HCS devices
        ---------------------------------------
        PICkit 2 Pin             HCS Device Pin
        (2) Vdd                  8
        (3) GND                  5
        (5) PGC                 /3            HCS20x, 320
                                \3 -or- 4     HCS30x, 36x
        (4) PGD                  6
        (1) VPP                  2            HCS360, 361 only


MCP250xx CAN Devices
--------------------
MCP25020       MCP25025 
MCP25050       MCP25055 

!!IMPORTANT!! - MCP250xx devices are OTP and can only be
                programmed once.

        Connections for MCP250xx devices
        ---------------------------------------
        PICkit 2 Pin             MCP Device Pin (DIP)
	(1) Vpp                  11 Vpp
        (2) Vdd                  14 VDD
		- The MCP device MUST be powered from PICkit 2!
        (3) GND                  7 Vss
        (4) PGD                  5 DATA
        (5) PGC                  6 CLOCK



Serial EEPROM Devices
---------------------
NOTE: Other supported voltage grades are listed in parentheses
      next to the device.  Select the "LC" part number to program
      these other voltage grades.

11LC010 (AA) 
11LC020 (AA) 
11LC040 (AA) 
11LC080 (AA) 
11LC160 (AA) 

24LC00   (AA)(C)                  25LC010A (AA) 
24LC01B  (AA)                     25LC020A (AA) 
24LC02B  (AA)                     25LC040A (AA) 
24LC04B  (AA)                     25LC080A (AA) 
24LC08B  (AA)                     25LC080B (AA) 
24LC16B  (AA)                     25LC160A (AA) 
24LC32A  (AA)                     25LC160B (AA) 
24LC64   (AA)(FC)                 25LC320A (AA)
24LC128  (AA)(FC)                 25LC640A (AA) 
24LC256  (AA)(FC)                 25LC128  (AA) 
24LC512  (AA)(FC)                 25LC256  (AA) 
24LC1025 (AA)(FC)                 25LC512  (AA) 
                                  25LC1024 (AA) 


93LC46A/B/C  (AA)(-C) 
93LC56A/B/C  (AA)(-C) 
93LC66A/B/C  (AA)(-C) 
93LC76A/B/C  (AA)(-C) 
93LC86A/B/C  (AA)(-C) 

        Connections for 11LC devices
        ---------------------------------------
        PICkit 2 Pin             11LC Device Pin (DIP)
        (2) Vdd !                8 Vcc
        (3) GND                  4 Vss
        (6) AUX                  5 SCIO

	! 11LC devices may not program properly below 3.6V VDD.
          This is a limitation of the PICkit 2 AUX IO pin.


        Connections for 24LC devices
        ---------------------------------------
        PICkit 2 Pin             24LC Device Pin (DIP)
        (2) Vdd !                8 Vcc
        (3) GND                  4 Vss
        (5) PGC                  6 SCL (driven as push-pull)
        (6) AUX                  5 SDA (requires pullup)
                                 7 WP - disabled (GND)
                                 1, 2, 3 Ax pins
                                    Connect to Vdd or GND per
                                    datasheet and to set address

	! 24LC devices may not program properly below 3.6V VDD.
          This is a limitation of the PICkit 2 AUX IO pin.


        Connections for 25LC devices
        ---------------------------------------
        PICkit 2 Pin             25LC Device Pin (DIP)
        (1) VPP                  1 nCS
        (2) Vdd                  8 Vcc
        (3) GND                  4 Vss
        (4) PGD                  2 SO
        (5) PGC                  6 SCK
        (6) AUX                  5 SI
                                 7 nHOLD - disabled (Vdd)
                                 3 nWP - disabled (Vdd)


        Connections for 93LC devices
        ---------------------------------------
        PICkit 2 Pin             93LC Device Pin (DIP)
        (1) VPP                  1 CS
        (2) Vdd                  8 Vcc
        (3) GND                  5 Vss
        (4) PGD                  4 DO
        (5) PGC                  2 CLK
        (6) AUX                  3 DI
                                 7 PE - enabled (Vdd)
                                 6 'C' Device ORG 
                                    Set to select word size



-----------------------------------------------------------------
2. Operating System Support List
-----------------------------------------------------------------

This tool has been tested under the following operating systems:

Windows XP
Windows Vista 32-bit
Windows Vista 64-bit  (Preliminary Testing Only)


NOTE: In Windows Vista, the PICkit2.INI file may be found in
C:\Users\<username>\AppData\Local\VirtualStore\Program Files\Microchip\PICkit 2 v2


-----------------------------------------------------------------
3. Release notes V2.61.00
-----------------------------------------------------------------
New Features:
-------------
> Programmer-To-Go updated to support 3rd party PICkit 2 workalike
  devices with more Programmer-To-Go memory.

Bug Fixes:
-------------
> PIC18F97J60 programming bug in device file v1.60 fixed

> PIC24FJ programming problems introduced in software v2.60 fixed

> Configuration Editor display issue in some Asian editions of 
  Windows addressed.


-----------------------------------------------------------------
   Release notes V2.60.00
-----------------------------------------------------------------
New Features:
-------------
> Low Voltage Programming mode support
  Low voltage programming mode (LVP) is now supported for all
  PICkit 2 supported devices which feature this option.  Refer
  to the device datasheet and programming specification for
  information on the LVP programming mode.
  LVP programming mode is enabled by the menu
  "Tools > User LVP Program Entry"
  This can only be enabled in Manual Device Select mode.  When
  enabled, the text "LVP" in red will appear above the Device
  select combo-box.
  For devices using the PGM pin in LVP mode, PICkit 2 ICSP
  connector pin 6 AUX must be connected to the device PGM pin.

  NOTE: LVP mode may only be used with devices that *already* have
  the LVP configuration bit enabled.  Blank devices always have
  the LVP bit enabled.

  NOTE: For PIC24F--KA-- devices, this option becomes
  "Tools > Use High Voltage Program Entry"
  This mode MUST be used with PIC24F--KA-- devices that have the
  MCLR pin disabled (used as IO) or PICkit 2 will not be able to
  detect or program the device.

  NOTE: LVP may be used with PICkit 2 Programmer-To-Go.  Enable
  "Tools > User LVP Program Entry" before starting the 
  Programmer-To-Go wizard.

> New Configuration Word Editor
  The editor dialog is opened by clicking on the "Configuration"
  text nest to the configuration word values in the main PICkit 2
  display.
  The editor allows editing of the individual bits in the
  configuration words, but the user must reference the device
  datasheet for the meanings of each bit.
  The PICkit 2 software does not contain a database of the names
  of the configuration bitfields and the meaning of their values.

> New menus Tools > Display Unimplemented Config Bits
  This menu allows the user to choose how unimplemented
  Configuration Word bits are displayed.  In previous versions,
  unimplemented bits were always displayed as '0' value.  This 
  did not always match the way MPLAB IDE displayed these "don't
  care" bits.
  Options are to display as '0' (default), as '1', or as the
  value in the imported HEX file / as they were read from the 
  device.

> Faster PIC18F6xJxx, 8xJxx programming times

> In Manual Device Select mode, PICkit 2 will now verify the
  Device ID of the target device matches that of the selected
  device.  If no device ID is detected, an error is generated.
  If the Device ID of a different device is detected, the name
  of the matching device will be shown.
  
  NOTE: To turn this feature off, edit the pickit2.ini file and
        change the entry for DVER: to N.  Ex
        DVER: N


Bug Fixes:
-------------
> Fixed some instances of the PICkit 2 software hanging on Reads,
  during Verify, or Blank Check with some chipsets.

> When importing a hex file, PICkit 2 should no longer warn when
  unused configuration words (with no implemented bits) are not
  contained in the HEX file.
  The exceptions are some dsPIC30F words that contain reserved
  bits.  The warning may be ignored for these cases.

> Fixed an issue when programming PIC24FJ appplication code that
  self-modifies program memory.  Such code should no longer cause
  Verify during a Write operation to fail.

> Baseline and Midrange checksums should now be computed correctly
  when Code Protect (CP) is enabled.

> Fixed an issue where the silicon revision was sometimes
  displayed with junk in the upper word of the value.
  NOTE: silicon revision display is enalbed byt adding the INI
  file value "REVS: Y"

> Alert Sounds will now point to the correct location for the 
  default sounds if the software is not installed in the default
  location.

> Fixed an issue with the progress bar display during writes
  and verifies when using the PE with PIC24 and dsPIC33 devices.

> Fixed an issue where "Tools > Use VPP First Program Entry" may
  not have had any effect in Manual Device Select mode.

> Fixed an issue where the "Fail" alert sound was sometimes played
  when downloading a memory image to Programmer-To-Go even when
  the download was successful.

> When Manual Device Select mode is active, the entire Programmer
  menu is no longer inaccessible when no device has been selected.
  This allows Manual Device Select mode to be exited without 
  having to select a device first.

-----------------------------------------------------------------
   Release notes V2.55.02
-----------------------------------------------------------------
Bug Fixes:
-------------
> Updates the PIC32 Programming Executive to v0109.  The prior
  version caused problems programming some PIC32 devices.

See below for additional updates in V2.55.xx

-----------------------------------------------------------------
   Release notes V2.55.01
-----------------------------------------------------------------
Bug Fixes:
-------------
> Fixes a UART Tool update rate issue introduced in V2.55.00

See below for additional updates in V2.55.xx

-----------------------------------------------------------------
   Release notes V2.55.00
-----------------------------------------------------------------
New Features:
-------------
> Faster PIC24 & dsPIC33 programming
  PICkit 2 now supports use of the Enhanced ICSP programming mode
  using the Programming Executive (PE) for these devices.  In
  addition to faster programming times, use of the PE provides
  the following:
    PIC24H & dsPIC33F:
      - Includes the Device ID corruption errata workaround
      - Verify is done using a 16-bit CRC and is very quick
      - Blank Check is done in the PE and is very quick
    PIC24F:
      - Verify is still done by reading out the device and
        benefits from faster PE reads
      - Blank Check is still done by reading out the device and
        benefits from faster PE reads

  The PE is never used for devices with < 4096 instruction
  flash sizes.

  Use of the PE may be disabled with programming reverting
  to basic ICSP as used in previous releases as follows:
    PIC24H & dsPIC33:
      - Edit PICkit2.INI to set the following entry to 'N':
        PE33: N
      - To re-enable, edit the INI file and change the entry
        back to 'Y'
      - NOTE: Reverting to ICSP mode no longer protects against
        the Device ID corruption errata
    PIC24F:
      - Edit PICkit2.INI to set the following entry to 'N':
        PE24: N
      - To re-enable, edit the INI file and change the entry
        back to 'Y'

  NOTE: PICkit 2 Programmer-To-Go does not use the PE in this
        release, and still uses basic ICSP programming.

> Import/Export of binary (*.bin) files for serial EEPROMs
  When a serial EEPROM device is selected as the current device,
  *.bin binary files may be imported and exported in addition to
  *.hex files.

  NOTE: A file MUST have the .bin extension to be imported or
        exported as a binary file.  Any other extension will be
        treated as a hex format file

> The UART Tool Hex mode allows direct typing
  In HEX mode, when the display is selected hex characters may be
  typed directly.  When the first nibble is typed, it is shown
  below the display as "Type Hex : 0_" where '0' is the first nibble
  value.  When a second nibble character is typed, the byte is
  transmitted.  The first nibble may be cancelled by pressing ESC
  or typing any non-hex character.

> New menu option "Programmer > Alert Sounds.."
  This option brings up a dialog to optionally select and enable
  playing of WAV sound files on success, warning, and/or error
  events in the status window.  A default WAV sound for each is
  included with the installion in the "Sounds" subdirectory of the
  PICkit 2 program directory.


Bug Fixes:
-------------
> (Device file v1.53) Fixed an issue with PI32MX4xx device config
  masks not including USB configuration bits.

> (Device file v1.53) PIC18F no longer reports verify errors or
  reads certain locations improperly when ETBR table read protect
  bits are asserted.

> Fixed UART Tool issue where siginifcant amounts of received 
  data without a newline (ASCII mode) or transmission pause
  (HEX mode) would cause sluggishness and lockups of the UART
  Tool software.

> Unit ID maximum length set to 14 characters due to a firmware
  issue.

> Tools > Troubleshoot... dialog 30kHZ waveform is closer to
  30kHz and no longer has breaks in the waveform.  This issue
  originated with the firmware v2.3x update.

  NOTE: The test waveform is only intended to check the edge rise
        and fall rates of the PGx signals.  The waveform still
        contains significant jitter and is only approximately
        30kHz in frequency.

-----------------------------------------------------------------
   Release notes V2.52.00
-----------------------------------------------------------------

New Features:
-------------
> Menu option "Programmer > Clear Memory Buffers on Erase" allows
  the user to select whether or not the application device memory
  buffers (Program Memory, EEPROM Data, User IDs, and
  Configuration) are cleared to blank values or remain unchanged
  when an "Erase" operation is performed.  In prior versions, the
  behavior has been to always clear the buffers on an "Erase."


Bug Fixes:
-------------
> Fixed a multiple PICkit 2 support critical issue that was
  causing simultaneous use of multiple PICkit 2 units to fail.
  This fix requires firmware version 2.32.00 for the PICkit 2 OS.

> Fixed a dsPIC33/PIC24HJ Programmer-To-Go problem causing verify
  of devices to fail in Programmer-To-Go mode when Config 8 was
  not defined in the hex file.

> Fixed an issue causing the application to crash when
  programming PIC32 devices with completely blank Boot Flash.
  

-----------------------------------------------------------------
   Release notes V2.51.00
-----------------------------------------------------------------

New Features:
-------------
> Logic Tool dialog now allows the PICkit 2 VDD to be turned
  ON and OFF from the dialog via the "VDD On" checkbox.
  NOTE: the voltage value must still be set in the main form.

> UART Tool dialog now allows the PICkit 2 VDD to be turned
  ON and OFF from the dialog via the "VDD" checkbox.
  NOTE: the voltage value must still be set in the main form.
  The VDD checkbox will be disabled when the UART Tool is
  "Connected"
  To change the VDD setting, the UART Tool must be "Disconnected"

Bug Fixes:
-------------
> Fix for Programmer-To-Go hanging on download for program sizes
  greater than half the maximum allowed.

> Fix for PICkit 2 operational problems after exiting Programmer-
  To-Go mode.  (Including reading junk data, failure to program,
  and odd VDD behavior).

> Corrected UART Tool Custom Baud dialog size.

> Reads of PIC32 devices now display "done" at completion.

> Added 100ms delays to PIC32 Program Executive download to fix
  errors in some instances.


-----------------------------------------------------------------
   Release notes V2.50.02
-----------------------------------------------------------------

Bug Fixes:
-------------
> Fix to try to prevent the problem of PICkit 2 windows 
  "disappearing" by opening off screen.
  If a problem is still found (Pk2 shows up in taskbar, but no
  window is visible & it's not minimized) the best solution is
  to close the PICkit 2 application (right-click task bar icon 
  and select "Close"), then delete the INI file.

> Disables Tools > UART Tool and Tools > Logic Tool when no
  PICkit 2 unit is available

> Addresses issues with multiple PICkit 2 unit support:
  - When the selected unit is in bootloader, no longer asks twice
    to select a unit.
  - Fix for a "Download Failed" issue when updating a unit with
    other units connected to the PC.
  - After updating a unit's OS, it will open the dialog asking
    the user to select a unit (if more than 1 are present).
    After the update reset, the unit may enumerate with a
    different USB ID.

***    IMPORTANT NOTES WHEN USING MULTIPLE PICKIT 2 UNITS     ***
*                                                               *
*   1) When updating a unit's OS, it is strongly recommended    *
*      that it be the only unit connected.                      *
*   2) Never connect more than 1 PICkit 2 with firmware OS      *
*      earlier than v2.30.01.  It may crash the PICkit 2 app    *
*      and/or Windows.                                          *
*   3) Never connect more than 1 PICkit 2 in Bootloader mode    *
*      ("Busy" LED blinking slowly).  This may crash Windows.   *
*   4) Never update the firmware OS of a unit when another unit *
*      in bootloader mode is connected, or has OS earlier than  *
*      v2.30.00                                                 *
*   5) The PIckit 2 Programmer Application will detect up to a  *
*      maximum of 8 connected PICkit 2 units.                   *
*****************************************************************

-----------------------------------------------------------------
   Release notes V2.50.01
-----------------------------------------------------------------

Bug Fixes:
-------------
> Requires firmware v2.30.01 which fixes an issue with 24LC
  serial EEPROM reads.


-----------------------------------------------------------------
   Release notes V2.50.00
-----------------------------------------------------------------

New Features:
-------------
> PIC32 Device Support
  See section 2 of this file for specific part numbers supported.
  ASCII view options are not available for PIC32 devices, and
  Fast Programming is always enabled.

> Multiple PICkit 2 unit support on one PC
  Multiple PICkit 2 units may be used with multiple instances of
  the PICkit 2 Programmer application and with MPLAB.  For 
  example, one might be used in MPLAB as a debugger, a 2nd with
  the UART Tool, and a 3rd with the Logic Tool.  
  When opening the PICkit 2 application or selecting "Tools > 
  Check Communications" a dialog will open if multiple units are
  found.  The dialog lists all connected PICkit 2 units and their 
  assigned UnitID string, and is used to select a unit to use.
  Versions of MPLAB that are not aware of multiple units (v8.10
  and earlier) will use the PICkit 2 unit listed in the dialog
  as Unit# = 0.

> Multiple Window View option with resizable memory windows.
  New in v2.50.00 is the "View" menu, with two selectable view
  options:
  "Single Window" - this is the default window used by prior
                    versions.
  "Multi-Window"  - The PICkit 2 application window is divided
                    up into 3 windows:
                    Main Window - essentially, the top of the 
                                  Single Window display.
                    Program Memory - Program memory contents are
                                     displayed in a separate
                                     re-sizable window.
                    EEPROM Data - separate resizable window
                                  with device EEPROM contents.
  The EEPROM Data window is only displayed for devices which
  have internal EEPROM memory.  Both memory region windows may
  also be closed or minimized, reducing the screen footprint
  of the application to only the Main Window.  Close memory
  windows may be opened by selecting them under the "View" menu.
  The "Associate / Memory Displays in Front" option associates
  the windows so they move, minimize, and come into focus
  together.  It also shows memory displays always in front of
  the main window. If this option creates problems with a 
  particular display, it can be unchecked to disable it.

  NOTE: If a multi-window display problem occurs, or a window
  is "lost", the defaults can be restored by deleting the 
  PICkit2.INI file in the installation directory.

> PICkit 2 Programmer-To-Go
  This new functionality allows a memory image to be downloaded
  to the PICkit 2 and programmed later without a PC.  All PIC
  MCU families are supported with the exception of PIC32.  See
  "Help > Programmer-To-Go User Guide" for more information.

> Manual Device Select
  "Programmer > Manual Device Select" may used to select devices
  in all families from a drop-down box.  This can be useful for
  importing HEX files to view when no device is present, and to
  download a memory image for Programmer-To-Go when a device is
  not avaiable to connect.

> Logic Tool
  The Logic Tool, available under "Tools > Logic Tool" allows  
  the PICkit 2 to be used to provide stimulus to and monitor
  signals in a target circuit.  It also provides a 3-channel
  logic analyzer with complex trigger options.  See "Help >
  Logic Tool User Guide" for more information.

> UART Tool VDD
  When using the UART Tool, the PICkit 2 unit may supply VDD to
  the target circuit.  The UART tool is now documented in the 
  PICkit 2 User's Guide.

> Memory display select & copy data
  The contents of the Program Memory display and EEPROM Data
  display may be selected and copied.  Use the mouse with click
  and drag to select portions of the memory data.  Right click
  and select "Select All" or press ctrl-A to select the entire
  memory contents.  Right click and select "Copy" or press
  ctrl-C to copy the data to the clipboard.  The selected
  addresses and data are copied to the clipboard as tab-delimited
  text.
  This works with both View options.

> Display of HEX file Code / Data Protect settings
  When a hex file with Code Protect and/or Data Protect bits
  asserted in the Configuration bits is loaded, this is noted
  by displaying "Code/Data/All Protect" next to the Configuration
  Word(s) display.  Similarly, the protect settings are noted for
  Configuration read from a device.  Note that protection 
  settings in an imported hex file or read from a device may not
  be turned off in the application.

> MCP250xx CAN device programming support.  See section 2 for
  specific part numbers.  IMPORTANT: These devices are One-
  Time-Programmable (OTP) and CANNOT be reprogrammed.  Also,
  the devices MUST be powered from PICkit 2 during programming.

> PIC18 J-series programming change
  PICkit 2 v2.50 changes the way Configuration words are programmed
  to match the behavior of the MPLAB IDE.  When programming, the
  upper nibble of configuration words is always set to hex "F".
  When erasing, unused configuration bits are masked off to "0".

> PIC24FJ programming change
  PICkit 2 v2.50 changes the way Configuration words are programmed
  to match the behavior of the MPLAB IDE.  When programming, the
  upper byte of configuration locations is always set to hex "FF".
  Unimplemented bits are shown as '1' in the Program Memory
  window.

> PIC24HJ / dsPIC33FJ Programming change
  PICkit 2 v2.50 now always disables the JTAGEN configuration bit.

> The PICkit 2 application window remembers its last location on
  the screen when closed and re-opened.

> Warns if the hex file contains settings for some Configuration
  Words but not others.

> Now displays 16-bit device revisions when REVS: is enabled in
  the INI file.

> Preliminary Microsoft Windows Vista 64-bit OS support.


Bug Fixes:
-------------
> Fixed an issue where application could not open the Device File
  if it did not have write permissions in the application
  directory.

> Fixed display issues with DPI settings other than 96 (Normal)

> When the attached PICkit 2 is in Bootloader mode, the 
  "Help > About" dialog now correctly reports the bootloader
  version.

> Fixed an issue where the view mode was always reset to 
  "Hex Only"

> The "Write on PICkit Button" no longer programs endlessly when
  the button is held down.  It programs once and waits for the
  button to be released.

> Fixed issues with detection on PIC18 K-Series devices erasing
  or corrupting Midrange devices on startup or when selecting
  "Check Communication".

> Fixed issue with 16F5x baseline hex import, which could cause
  configuration verify errors when programming.


-----------------------------------------------------------------
   Release notes V2.40.00
-----------------------------------------------------------------

New Features:
-------------
> New menu option "Tools -> Use VPP First Program Entry"
  When selected, can allow PICkit 2 to connect to and program
  devices with configurations and code that intereferes with the
  ICSP signal pins.

  Symptoms that might require turning this option on include
  Writing a device and getting a 'Verification of configuration
  failed.' error, and not being able to connect to the device
  for further programming operations.

  NOTE that when this option is enabled, the target MUST be 
  powered from the PICkit 2 VDD pin.  It will not work with
  target-powered devices.

> VDD set value retained across application sessions.
  When the PICkit 2 Programmer application is opened, the value
  in the VDD set box when the application was last closed will be 
  restored.  However, if a part is detected from a device family
  different than the family that was active when the application
  was last closed, the VDD box value will not be restored and it
  will be set to a default value.

  For example:
     When the application was closed:
       PIC18F family was active, VDD was set to 3.1V

     Then when restarted:
        If PIC18F device is detected - VDD is restored to 3.1V

	If no device detected - VDD is restored to 3.1V
           (Family is defaulted to last used)

        If Midrange PIC16F device detected - VDD defaults to 5.0V
           (any family besides last used (18F), VDD set default)

> Calibrate Vdd & Set Unit ID
  This option under the "Tools" menu allows the PICkit 2 Vdd
  output voltage to be calibrated using a Volt Meter.  This also
  increases the accuracy of detected voltages for powered 
  targets.  Frequently accuracies within 2% can be achieved.
  NOTE: Since the PICkit 2 voltages are referenced to the USB
        voltage, the calibration may not be valid if the
        PICkit 2 is moved to another USB port or host PC.
  NOTE: The PICkit 2 VDD output high end is still limited by
        the USB voltage and the D4 diode drop.
  The calibration is stored in the PICkit 2 unit, so a unit
  will remain calibrated when used with the MPLAB IDE.

  A Unit ID may also be assigned to a PICkit 2 unit.  The
  PICkit 2 programmer application will identify the attached
  PICkit 2 unit with the ID in the application title bar.
  This can be useful in keeping track of and identifying
  multiple PICkit 2 units.

  NOTE: This menu option is not available if memory editing has
        been disabled with an "EDIT: N" entry in the INI file.
        See "Release notes V2.01.00" for more information.

> OSCCAL instruction verification.
  For devices with an OSCCAL oscillator calibration value
  instruction in the last location of program memory, the
  PICkit 2 GUI will indicate if an invalid instruction value
  is detected in the Device Configuration display.
  During a WRITE or ERASE operation, if an invalid OSCCAL value
  is detected, the application will warn the user and give them
  the option to abort the operation or continue.

> KEELOQ HCS part support.  

	HCSxxx File -> Import HEx Notes:
		The first line only may be imported from SQTP
                *.NUM files generated by the KEELOQ tool in 
                MPLAB.

        Connections for HCS devices
        ---------------------------------------
        PICkit 2 Pin             HCS Device Pin
        (2) Vdd                  8
        (3) GND                  5
        (5) PGC                 /3            HCS20x, 320
                                \3 -or- 4     HCS30x, 36x
        (4) PGD                  6
        (1) VPP                  2            HCS360, 361 only

> Serial EEPROM support:

  24LC I2C bus devices:
       Bus Speed-
                400kHz with Tools -> Fast Programming checked
                100kHz with Tools -> Fast Programming unchecked

                NOTE: Bus pullups are required for all
                      programming operations.  400kHz requires
                      2k Ohm pullups.

        Ax Chip Select checkboxes-
                These are only enabled for devices that support
                address chip selects, and allow programming of
                multiple devices on the same bus.

        Connections for 24LC devices
        ---------------------------------------
        PICkit 2 Pin             24LC Device Pin (DIP)
        (2) Vdd                  8 Vcc
        (3) GND                  4 Vss
        (5) PGC                  6 SCL (driven as push-pull)
        (6) AUX                  5 SDA (requires pullup)
                                 7 WP - disabled (GND)
                                 1, 2, 3 Ax pins
                                    Connect to Vdd or GND per
                                    datasheet and to set address

  25LC SPI bus devices:
        Bus Speed-
                ~925kHz with Tools -> Fast Programming checked
                ~245kHz with Tools -> Fast Programming unchecked

        Connections for 25LC devices
        ---------------------------------------
        PICkit 2 Pin             25LC Device Pin (DIP)
        (1) VPP                  1 nCS
        (2) Vdd                  8 Vcc
        (3) GND                  4 Vss
        (4) PGD                  2 SO
        (5) PGC                  6 SCK
        (6) AUX                  5 SI
                                 7 nHOLD - disabled (Vdd)
                                 3 nWP - disabled (Vdd)

  93LC Microwire bus devices:
        Bus Speed-
                ~925kHz with Tools -> Fast Programming checked
                ~245kHz with Tools -> Fast Programming unchecked

        Connections for 93LC devices
        ---------------------------------------
        PICkit 2 Pin             93LC Device Pin (DIP)
        (1) VPP                  1 CS
        (2) Vdd                  8 Vcc
        (3) GND                  5 Vss
        (4) PGD                  4 DO
        (5) PGC                  2 CLK
        (6) AUX                  3 DI
                                 7 PE - enabled (Vdd)
                                 6 'C' Device ORG 
                                    Set to select word size

> UART Tool
  The UART Communication Tool, available under the "Tools" menu,
  allows the PIckit 2 to be used as a serial UART interface for 
  communicating with a microcontroller. Potential uses include:
  - Display debug text output from the microcontroller 
  - Logging microcontroller data to a text file
  - Developing & debugging a serial UART interface
  - Sending commands to the microcontroller during development

  The PICkit 2 unit connects as follows:
        PICkit 2 Pin             Target UART
        (1) VPP                  
        (2) Vdd                  Vdd (Vcc)
        (3) GND                  GND
        (4) PGD                  TX - inverted, logic level
        (5) PGC                  RX - inverted, logic level
        (6) AUX                  

  IMPORTANT CONNECTION NOTES:
        PICkit 2 cannot supply Vdd when using the UART Tool.
        The PICkit 2 Vdd pin MUST be connected to the target UART
              Vdd or it will not work.
        TX & RX signals are inverted (Start Bit = GND, Stop Bit =
              Vdd) at logic levels.  The PICkit 2 CANNOT be 
              connected to RS-232 +/- 12V signals.

  The UART Tool window may be resized (expanded) and the PICkit 2
        BUSY LED acts as an activity light (for both RX & TX)

  Baud Rate:
        The baud rate is selectable from the dropdown box in the
        upper left corner.  Common rates are included, however
        by selecting "Custom..." any baud rate from 150 to
        38400 in 1 baud increments may be used.

  Connect/Disconnect:
        The baud rate can only be changed when Disconnect is
        selected.  Data will only be received and trasmitted when
        Connect is selected.

  Mode : ASCII
        - Received bytes are displayed as ASCII characters.
        - Bytes are transmitted by typing on the keyboard,
          using the String Macros, or pasting text.
        - Transmitted data is not displayed unless "Echo On"
          is checked.
        - If "Append CR + LF" is checked, then when "Send" is
          clicked for a String Macro, an extra two bytes
          comprising of a Carriage Return (0x0D) and Line Feed
          (0x0A) are sent after the string data.
        - A New Line is displayed when both a Carriage Return
          and Line Feed are received.  Individually, they will
          display as a box character.
  Mode : Hex
        - Received bytes are displayed as hex values preceded by
          "RX: "
        - Bytes are transmitted only by using the Hex Sequence
          boxes.  Transmitted bytes are always displayed as hex
          values preceded by "TX: "

  String Macros / Hex Sequences:
        These text boxes allow strings of ASCII characters or
        hex bytes to be entered and sent all at once by clicking
        the "Send" button.  They can also be used for frequently
        used string commands.  
        In ASCII mode, each box has a limit of 60 characters.
        In Hex mode, each box has a limit of 48 bytes.

  Wrap Text:
        In either mode, determines whether text without a newline
        will wrap at the right edge of the display area, or be
        displayed on a single line with a horizontal scroll bar.
        The display will keep about 200 lines of received text in
        the buffer.

  Log to File:
        Allows received & transmitted data to be saved to a text
        file as it appears in the display area.  Only data
        received and transmitted after the logfile is opened will
        be saved.  Existing data in the display is not saved.
        While logging data, the button turns green.  To stop
        logging data and close the file, click the button again.
  
  Clear Screen:
        Clears the display buffer.  Does not affect log file.

  Exit UART Tool:
        Returns to the PICkit 2 Programmer interface.


> New Operating System firmware v2.10.  If you are using a 
  version of MPLAB IDE prior to 7.62, it will want to reprogram
  the firmware to an earlier version.  If you switch between
  applications frequently and want to prevent this, do the 
  following -
  Copy the file: 
  C:\Program Files\Microchip\PICkit 2 v2\PK2V021000.hex 
  into: 
  C:\Program Files\Microchip\MPLAB IDE\PICkit 2\ 


Thanks to Lanchon, xiaofan, and many other users & Microchip
forum members for all their suggestions, comments, and feedback.

-----------------------------------------------------------------
   Release notes V2.30.00
-----------------------------------------------------------------

New Features:
-------------
> File - Import Hex and File - Export Hex are now affected by the
  memory region checkboxes for parts with EEPROM data.  During
  import, unchecked regions will not be imported and the existing
  memory buffer contents will be unchanged (unless a different
  part is detected.)  During export, unchecked memory regions
  will not be included in the saved hex file.
  This allows only EEPROM data to be imported from a hex file, 
  for example, or all data except EEPROM.  On export, it allows
  creation of a hex file without EEPROM data, or with only 
  EEPROM data.

> For Vdd = 3.6V Max parts, now sets voltage to a nominal 3.3V.

> New view format for Program Memory and EEPROM Data
  "Word ASCII" displays the same as"Hex+ASCII" in prior releases.
       ASCII characters are displayed in the same order bytes
       appear in the memory word.  Ex:
       '694D 7263 636F 6968   iM    rc    co    ih'
  "Byte ASCII" - new display format where ASCII characters are
       displayed in memory byte order with a space in between.
       This gives better readability to strings in memory. Ex:
       '694D 7263 636F 6968  M i   c r   o c   h i'

> Low Vdd programming of selected Midrange parts, all PIC18F, and
  most dsPIC30F parts.
   -----------------------------------------------------
  |NOTE: Low Vdd programming WILL NOT WORK if any       |
  |config code, data, or write protect bits are active! |
   -----------------------------------------------------
   -----------------------------------------------------
  |NOTE: This is not the same as LVP programming, which |
  |uses the PGM pin for program mode entry.             |
   -----------------------------------------------------
  Previously, programming always used a Bulk Erase which required 
  a minimum Vdd of 4.5V for Midrange parts, many PIC18F parts, 
  and dsPIC30F parts.
     Midrange: Some Midrange parts support a flash row erase
               process at below 4.5V.  These devices are now
               supported for programming at Vdd < 4.5V using
               the row erase.
               See the device support list for supported parts.

     PIC18F:   All PIC18F parts now program down to 3.0V.  Some
               will program at lower voltages; the user will be 
               warned if the voltage is too low.

     dsPIC30F: Most dSPIC10F devices now program down to 3.0V.
               The exceptions are the dsPIC30F5011 and 
               dsPIC30F5013.

   -----------------------------------------------------
  |NOTE: The [ERASE] button/menu still uses Bulk Erase  |
  |only.                                                |
   -----------------------------------------------------
  Therefore, [ERASE] cannot be used at voltages below the Bulk
  Erase minimum Vdd.
  Parts that support the low Vdd programming can be left erased
  by following these steps:
  1) Connect to the device
  2) Select menu Device Family -> {family of device in use}
        This clears all buffers to the erased state
  3) Click [WRITE]
  4) The device is now in a completely erased state.
     (NOTE this will not work if any protect bits are set)


Bug Fixes:
----------
> Fixed issue with PIC18F config bit WRTC preventing the 
  programming of CONFIG7.
> Fixed issue with EEPROM read.  When PICkit 2 was started with
  no device attached, then a device was attached and the first
  operation was a read, EEPROM Data memory would not be read.
  Now corrected.


Firmware Update:
----------------
> This version requires firmware v2.02.00.  MPLAB IDE 
  version 7.60 and prior will detect the new version and ask to
  download v2.01.  However, it is safe to use v2.02 with these
  MPLAB IDE releases.


-----------------------------------------------------------------
   Release notes V2.20.04
-----------------------------------------------------------------

Bug Fixes:
----------
> Fixed handle issue causing crashes with Program On PICkit Button

-----------------------------------------------------------------
   Release notes V2.20.00
-----------------------------------------------------------------

New Features:
-------------
> dsPIC30 support
> PIC18LF_J_ support:
	NOTE: If these devices do not have VDDCORE being supplied
              by an external regulator from the general VDD (that
	      is able to handle 3.6 Volts), but VDDCORE is
              powered directly from the PICkit 2 VDD output, the
              following precautionary steps must be taken.  These
              prevent a potentially damaging overvoltage on
              VDDCORE:
                Step 1 - Do not connect the target device to the
                         PICkit 2 unit when opening the
                         programming application.
                Step 2 - After the application is opened, select
                         Menu "Device Family > PIC18F_J_" (if 
                         this is not the current family)
                Step 3 - Change the "VDD PICkit 2" voltage box
                         to between 2.5 and 2.7 Volts
                Step 4 - Connect the target device to the
                         PICkit 2 unit.
                Step 5 - Detect the device by either trying a 
                         programming operation (such as Read)
                         or again selecting menu 
                         "Device Family > PIC18F_J_"


Bug Fixes:
----------
> Fix issue with with Erase voltage warning dialog and Auto-
  Import-Write that was causing an exception.

-----------------------------------------------------------------
   Release notes V2.11.00
-----------------------------------------------------------------

New Features:
-------------
> Imports/exports new MPLAB PIC24HJ and dsPIC33 HEX file format

Bug Fixes:
----------
> Baseline & Midrange hex files imported with Code Protect config
  bit asserted now display correct checksum
> PIC18F hex files imported with add Code Protect config bits
  asserted now display correct checksum
  NOTE: PIC18F hex fies imported with only some (not all) CP bits
        will display a checksum not matching MPLAB
> Code Protect masks for PIC24HJ and dsPIC33 parts updated

-----------------------------------------------------------------
   Release notes V2.10.01
-----------------------------------------------------------------

New Features:
-------------
> 44-Pin Demo Board User's Guide included with installation and
  linked to under "Help" menu.
> 'Tools > Write on PICkit Button' state now saved in INI file.
> If an Auto-Import-Write fails,the button is now left enabled
  to more easily retry.

Bug Fixes:
----------
> Auto-Import-Write function no longer locks up if VDD is set to
  "Force Target" and no target power is present.  Also, when set
  to "Auto-Detect", popup dialogs indicating new power mode are
  suppressed.

-----------------------------------------------------------------
   Release notes V2.10.00
-----------------------------------------------------------------

New Features:
-------------
> dsPIC33FJ support and PIC24HJ support
> Better support of devices with configuration set for /MCLR OFF
> Troubleshooting Wizard for assistance debugging ICSP
  connections.
> Added Programmer -> Hold Device in Reset and /MCLR checkbox
  Allows control over device /MCLR signal.
  (Active low when selected, tri-state when not)
> Frequently used menu items have keyboard shortcuts.
> Auto functionality added to Import HEx & Write Device.
  When selected hex file is updated, it is automatically imported
  and written to the device.


-----------------------------------------------------------------
   Release notes V2.01.00
-----------------------------------------------------------------

New Features:
-------------
> PIC24FJ part support
> Significantly faster programming times for PIC18F
> ASCII views of data
> File menu import history
> Verify on Write can be turned off
> Verify and Blank Check will stop on the first error, and report
  the location of the error.
> Begin programming on PICkit 2 button.
> Separate enables for Program Memory and Data Memory 
  Code Protects.
> Target VDD support may be set to 
	Auto-Detect
	Always Powered from PICkit 2
	Always Powered by Target Board
> Selectable programming speed for heavily loaded ICSP lines
> Pop-up with memory address on program memory and data memory
> Menu settings are remembered the next time the application is 
  started.

> Memory Region Selection:

    The following programmer functions will always operate on all
    regions of device memory, regardless of memory region select
    checkboxes:
        Programmer -> Erase
        programmer -> Blank Check

    For devices with EEPROM Data Memory, the checkboxes will 
    affect the following programmer functions:
	Programmer -> Read      (including Read & Export button)
	Programmer -> Write     (including Import & Write button)
	Programmer -> Verify

    The regions affected are as follows:

        Program Checkbox:   EE Data Checkbox:    Read/Write/Verify:
        -----------------   -----------------    ------------------
        [X]                 [X]                  All regions
        [X]                 [ ]                  Program Memory, UserIDs, 
                                                 Config Word(s)*
        [ ]                 [X]                  EE Data only*
	[ ]                 [ ]                  - not allowed -

        *NOTE that region selections may not work properly if 
         code protect, data protect, or write protects are  
         presently active in the device.


Program Memory and EEPROM Data editing:
---------------------------------------
By default, program memory and EEPROM data may be edited in the 
display windows.
To disable this feature, edit the INI file
C:\Program Files\Microchip\PICkit 2 v2\PICkit2.ini
(Created when the application is first run)

Change the "EDIT" parameter to "N":
EDIT: N




