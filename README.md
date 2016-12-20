# CHIP i2c and GPIO in Gforth
C.H.I.P. i2c and GPIO low level and other code to access devices with Gforth

* CHIP_Gforth_i2c.fs
  * This file makes low level reading and writing to CHIP i2c work in gforth.  At this writing I have confirmed on headless OS 4.4 that U14 pin25 and pin26 ( TWI2-SDA(i2c-2-SDA) & TWI2-SCK(i2c-2-SCL) ) work without a device overlay added to the OS.  I talked to a BMP180 device at i2c address #77 with the below gforth software for this sensor.  Use this as follows at the command line:

  `sudo gforth CHIP_Gforth_i2c.fs`

  * Note there are some warning messages generated at this moment of compiling that i will need to remove.  Some changes in the shared library method seems to have caused this.  I will deal with this when i can but for now it does work on my CHIP hardware.
  * I added an ability to force an i2c connection to CHIPi2copen word.  Realize this could damage devices and or confuse the kernel so use with caution!

* BMP180-object.fs
  * This file is an object that can be used to talk to the BMP180 sensor provided it is connected to U14 pin25 and pin26 for i2c-2 connection.  I have confirmed at this writing that this does work with the above CHIP_Gforth_i2c i2c low level words.  Use this as follows at the command line:

  `sudo gforth BMP180-object.fs`

  Now once in gforth to read the sensor do the following:

  ```
  bmp180-i2c heap-new constant mybmp180
  mybmp180 display-tp
  mybmp180 read-temp-pressure throw cr ." humd:" . cr ." temp:" .
  ```

  This example shows two ways to read the data from the object called bmp180-i2c.

* CHIP_battery_lvl.fs
  * This file talks to the AXP209 device on the CHIP board to get the battery voltage if you have on connected.  Use this example as guidance to talk to the AXP209 device for other information.  I got the idea from the file at /usr/bin/battery.sh found on headless chip os 4.4.  Note the AXP209 device seems to be on i2c bus address 0 and this bus is used by the kernel.  So to get around this i added the ability to force an i2c connection when opening the handle with the CHIP_Gforth_i2c.fs code.  This force a connection method could cause damage to i2c devices and or confuse the kernel so use this with care.

* clean-myGforth_i2c-libs
  * This file is a simple bash script to delete the shared library files that are created when you use CHIP_Gforth_i2c.fs the first time.  Use this file to remove the shared library when an update happens on this git repository for the CHIP_Gforth_i2c.fs library.

* [i2c-information.md](i2c-information.md)
  * Information file about CHIP and i2c!
