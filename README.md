PICkit- software suite
======================

You can use your trusty old PICkit2 and PICkit3 with the new Microchip PIC microcontrollers with PICkitminus program. It adds support for many chips not covered by the original PICkit software. This repository has the GUI version of the software. See my another repository for the command line version, pk2cmd. Both will automatically detect PICkit2, PICkit3 and PKOB. The PICkit3 firmware is improved and now supports all the features as PICkit2 does.

![pickit2_pic18f26k83](https://github.com/user-attachments/assets/9ed3ad52-864f-4cc0-949c-1b05b7e3892c)

Features
--------
- Supports 1540 devices, inclding nearly all, if not all, 8-bit PIC microcontrollers which use ICSP
- Supports PICkit2 and PICkit3 programmers, including clones and derivatives like PKOB, PICkit3.5 or [PK2M](http://kair.us/projects/pk2m_programmer/index.html)
- Improved auto detection of parts
- Programmer-to-go support with PICkit2 and PICkit3
- Programmer-to-go works with MSB1st families
- Optimized programming scripts for MSB1st families to reduce write and verify times
- Improved blank section skipping for write and verify, to further reduce programming times
- [Improved operation](https://forum.microchip.com/s/topic/a5C3l000000MdWiEAK/t381995) with PICkit3 clones
- [UART tool also for PICkit3](https://protoncompiler.com/index.php/topic,1616.0.html)
- UART possible to use also with other software [by creating a virtual COM port](http://kair.us/projects/pickitminus/pickit2_and_pickit3_as_virtual_com_port.html)
- [SPI FLASH device support](http://kair.us/projects/pickitminus/program_spi_flash_devices_with_pickit2_and_pickit3.html)
- GUI software works on Windows 2000, XP, 7, 10, 11
- Command line software works on Windows XP, 7, 10, 11, Linux and MacOS
- Retains all the good features from original Microchip PICkit2 and PICkit3 stand-alone software

Notes on PICkit3 and PKOB
-------------------------
The PICkit3 and PKOB (PICKit On Board) require a special 'scripting' firmware, just like the Microchip original PICkit3 standalone GUI software. When you start the GUI software, it guides you to update the firmware if needed. The pk2cmd doesn't yet support firmware updates for PICkit3 or PKOB.

When you want to use MPLAB, MPLAB-X or MPLAB-X IPE again, you must revert the PICkit3 to bootloader. To do this, select 'Revert to MPLAB mode' from Tools menu. Then start MPLAB(-X), and it will update the correct firmware for MPLAB usage. If you don't do this, you will get all kind of errors when trying to use your PICkit3 or PKOB with MPLAB.

PKOB operation has been tested with the following development boards:

- Curiosity PIC32MX470 Development Board (DM320103)
- PIC24F Curiosity Development Board (DM240004)
- Microstick II SK (DM330013-2)
- Explorer 16/32 Development Board (DM240001-2)

Please Note that all new devboards have PKOB4 or some other solution, those are not supported. Also many older boards have been updated to new revision. For example Curiosity HPC board (DM164136) originally had PKOB (based on PICkit3), but revision 2 has PKOB4 (based on PICkit4). The easiest way is to look at the microcontroller type on the devboard. If it is PIC24FJ256GB106, it is very likely PKOB, and probably will work.

Thanks
------
I haven't developed these software all by myself. The biggest part has of course been Microchip's original work, and all the contributions they had received from PICkit2 users. In addition to that, I have used work from other people. My thanks go to all contributions listed below:

- bequest333 for initially adding support for MSB 1st chips
- PICkitPlus team for providing updated device file until 2020
- dougy83 for creating device file editor
- Miklós Márton for adding PICkit3 support to pk2cmd
- timijk, scasis and TrevorW for adding support for all PIC32MX
- Adem Gdk for adding some SPI FLASH devices and testing SPI FLASH support
- Jaren Sanson for tool which adds some PIC24 devices
- All people who have sent me bug reports
