Readme for the PICkit(TM) 3 Programmer Application with scripting support

Release: v3.10.00
Device File:  v1.62.14

Updated: 10 June 2013

-------------------------------------------------------------------------------
Table of Contents
-------------------------------------------------------------------------------
1. Introduction
2. Installing with the PICkit 3 Programmer Application
3. Working with the PICkit 3 Programmer Application
4. Building the Firmware
5. Known Limitations
6. Supported Devices
7. Operating System Support List
8. Release notes

-------------------------------------------------------------------------------
1. Introduction
-------------------------------------------------------------------------------
This release of software/firmware allows the PICkit 3 to be compatible with the
scripting framework that was originally developed for the PICkit 2. This allows
sharing device files and application support between PICkit 2 and PICkit 3.

The software adds other features that were only available on the PICkit 2 such
as the logic analyzer, logic output, and auto detection of devices. Future
support is also planned for UART and other features not available in this
release.

Since PICkit 3 is supported under MPLAB(R) IDE with non-scripting firmware,
special firmware is needed to understand PICkit 2 scripts. A bootloader is used
to switch seamlessly between both operating systems.

-------------------------------------------------------------------------------
2. Installing with the PICkit 3 Programmer Application
-------------------------------------------------------------------------------
The PICkit 3 Programmer Application with scripting support is a modified version
of the PICkit 2 programmer application that was specially modified to work with
PICkit 3 scripting firmware.

To install the Application, unzip the contents of the zip file:
<PICkit3_scripting_app_v03.00.zip> into a temporary directory, then run the
install.exe and follow the installation wizard.

The .NET framework version 4.00 is required. You can download it at:

http://www.microsoft.com/download/en/details.aspx?id=17718 (standalone)

or

http://www.microsoft.com/download/en/details.aspx?id=17851 (web version)

-------------------------------------------------------------------------------
3. Working with the PICkit 3 Programmer Application
-------------------------------------------------------------------------------
The GUI will recognize MPLAB IDE compatible firmware when started with a
PICkit 3 connected that is loaded with MPLAB IDE compatible firmware.
A message indicating that an OS update is needed will be displayed.
To change to PICkit 3 Programmer Application firmware, choose
<Tools/Download PICkit Operating System> and then point to the path where an OS
file and a bootloader file reside. This is typically in the same place where
the PICkit 3 scripting executable resides.

An OS file name format is <PK3OSxxxxxx.hex> where xxxxxx is the version number.
The bootloader hex file name format is <PK3BLxxxxxx.hex> where xxxxxx is the
version number. Once an OS is downloaded, the GUI will try to reestablish
communication.

Please be aware that this OS is not compatible with MPLAB IDE, so to be able to
talk to MPLAB IDE again, <Tools/Revert to MPLAB mode> will need to be selected.
This reverts the PICkit 3 to bootloader mode so that MPLAB IDE can update the
PICkit 3 with MPLAB IDE compatible firmware.

If an external programmer is used to program the firmware image, the bootloader
should also be programmed into the device to allow updating of firmware or
restoring MPLAB compatibility. A complete firmware image <PK3IMGxxxxxx.hex>
is bundled with firmware source and the application. It includes startup code,
the bootloader, and the main OS.

If the GUI is used, it will automatically program the bootloader to flash memory
and will remain resident until you switch to MPLAB IDE mode and update
the firmware.

-------------------------------------------------------------------------------
4. Building the Firmware
-------------------------------------------------------------------------------

Below are the steps to build a firmware image which can be used with the GUI.

===========================================================================
NOTE:	These steps are only necessary if the firmware is modified by the
        user. A pre-built firmware image <PK3IMGxxxxxx.hex> is bundled with
		this release for use with an externam programmer, and a pre-built
		OS image <PK3OSxxxxxx.hex> is bundled for use with the GUI.
		xxxxxx is the version number.
===========================================================================

===========================================================================
NOTE:   An MPLAB C30 compiler later than 3.30 is recommended.
===========================================================================

------------------------------------
Building a complete firmware image:
------------------------------------

These steps will produce a hex file that can be used for programming the
PICkit 3 with an external programmer or for restoring the scripting firmware if
the image gets corrupted.

1- Unzip the firmware package file PICkit3_scripting_fw.zip
2- Open MPLAB IDE.
3- Go to <Configure/Settings/Projects> and make sure that "Use one-to-one
   project-workspace model is unchecked.
4- Open the workspace: <PICkit3.mcw> from the unzipped directory. You should see
   two projects loaded <branch.mcp> and <PICkit3OS.mcp>
5- Go to <Configure/Setting/Program Loading> and make sure "Clear program memory
   upon loading a program" is unchecked. This should be already saved in the
   workspace. It allows the output of both projects to reside in MPLAB memory
   side by side when building.
6- Build <branch.mcp> and <PICkit3OS.mcp> by right clicking on each project name
   in the project window and choosing "Build All".
7- After building, click on File/Import and point to the unzipped firmware
   directory. Select the file PK3BLxxxxxx.hex.
8- It is recommended verifying that the proper sections are loaded into memory,
   by opening View/Program Memory and checking that the different sections of
   code are loaded. There should be some data loaded (a goto instruction) at
   the Reset address 0. The OS is loaded starting at 0x800 and the bootloader
   at 0xF000. CTRL-G can be used to go jump to each of those addresses and
   verify that some data is loaded there.
9- Finally do a File/Export to export an image. Make sure Program Memory from
   0 to 0x2abf6 is selected as well as Configuration bits. This exported image
   can be used for programming with an external programmer.

------------------------------------
Building the OS only:
------------------------------------

These steps will produce an OS file that is suitable for downloading into the
PICkit 3 using the GUI.

1- Repeat steps <1-6>. Keep in mind that building <branch.mcp> is not necessary
   for this step.
2- A hex file PICkit3OS.hex will be produced under the following path:
   <firmware unzip directory>\PICkit3OS\obj
3- Copy this file into the same place where the GUI executable and device file
   reside.

-------------------------------------------------------------------------------
5. Known Limitations
-------------------------------------------------------------------------------
- This release does not include UART support. This will be added in a future
  release.
- Programmer-To-Go is not supported under this software. To use this feature,
  please use the MPLAB IDE.
- This release does not support automatic generation of an OSCCAL value.

-------------------------------------------------------------------------------
6. Device Support
-------------------------------------------------------------------------------

=================================================================
= NOTE: This list shows support for the PICkit 3 Programmer     =
= software application.  It does not show support for using the =
= PICkit 3 within MPLAB IDE.  For a list of MPLAB supported     =
= parts, see the MPLAB IDE PICkit 3 Readme.                     =
= (Typically in C:\Program Files\Microchip\MPLAB IDE\Readmes)   =
=================================================================


* Indicates new parts supported in this release with v1.61 of the
  device file.

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
PIC16F72
PIC16F73        PIC16F74        PIC16F76        PIC16F77
PIC16F716
PIC16F737       PIC16F747       PIC16F767       PIC16F777
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
PIC32MX340F512H
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
        PICkit 3 Pin             HCS Device Pin
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
        PICkit 3 Pin             MCP Device Pin (DIP)
	    (1) Vpp                  11 Vpp
        (2) Vdd                  14 VDD
		- The MCP device MUST be powered from PICkit 3!
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

> 11LC UNI/O bus devices:

        NOTE: The UNI/O (llLC) Serial EEPROM devices require the following PICkit 3
              hardware consideration or change to work properly:

              Application pull-up resistor needs to be 9.1k ohm or less
              or
              Remove R50 from the PICkit 3.

        Connections for 11LC devices
        ---------------------------------------
        PICkit 3 Pin             11LC Device Pin (DIP)
        (2) Vdd                  8 Vcc
        (3) GND                  4 Vss
        (6) PGM(LVP)             5 SCIO

> 24LC I2C bus devices:
		Bus Speed-
                400kHz with Tools -> Fast Programming checked
                100kHz with Tools -> Fast Programming unchecked

        NOTE: Bus pullups are required for all
              programming operations.  400kHz requires
              2k Ohm pullups.

		NOTE: The I2C (24LC) Serial EEPROM devices require the following PICkit 3
		      hardware changes to work properly:

		      Remove TR3 from the PICkit 3.
		      Remove R50 from the PICkit 3.

        Connections for 24LC devices
        ---------------------------------------
        PICkit 3 Pin             24LC Device Pin (DIP)
        (2) Vdd                  8 Vcc
        (3) GND                  4 Vss
        (5) PGC                  6 SCL (driven as push-pull)
        (6) PGM(LVP)             5 SDA (requires pullup)
                                 7 WP - disabled (GND)
                                 1, 2, 3 Ax pins
                                    Connect to Vdd or GND per
                                    datasheet and to set address

> 25LC SPI bus devices:
        Bus Speed-
                ~925kHz with Tools -> Fast Programming checked
                ~245kHz with Tools -> Fast Programming unchecked

        Connections for 25LC devices
        ---------------------------------------
        PICkit 3 Pin             25LC Device Pin (DIP)
        (1) VPP                  1 nCS
        (2) Vdd                  8 Vcc
        (3) GND                  4 Vss
        (4) PGD                  2 SO
        (5) PGC                  6 SCK
        (6) PGM(LVP)             5 SI
                                 7 nHOLD - disabled (Vdd)
                                 3 nWP - disabled (Vdd)


        Connections for 93LC devices
        ---------------------------------------
        PICkit 3 Pin             93LC Device Pin (DIP)
        (1) VPP                  1 CS
        (2) Vdd                  8 Vcc
        (3) GND                  5 Vss
        (4) PGD                  4 DO
        (5) PGC                  2 CLK
        (6) PGM(LVP)             3 DI
                                 7 PE - enabled (Vdd)
                                 6 'C' Device ORG
                                    Set to select word size

-----------------------------------------------------------------
7. Operating System Support List
-----------------------------------------------------------------

This tool has been tested under the following operating systems:

Windows XP
Windows 7 64-bit

-----------------------------------------------------------------
8. Release notes V3.01.00
-----------------------------------------------------------------

New Features:
-------------
> Added Serial EEPROM Device support.