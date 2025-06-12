12. Jun 2025
Release Notes for PICkitminus Microcontroller Programmer
PICkitminus V3.28.00
Device File V2.64.33

Number of supported devices: 1588

This release comes with new PICkit firmware V2.32.02
It adds commands needed for some PIC24/33EP families

KNOWN ISSUES / LIMITATIONS
- Programmer-to-Go not supported for PIC24/dsPIC33 parts with
  Customer OTP memory
- Programming of parts with dual partition flash is supported
  only in single partition mode
- OTP config registers in PIC18F MSB1st parts are not written
  to prevent accidental write. Please contact me if you need this
  feature