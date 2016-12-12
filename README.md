# CHIP_i2c_GPIO
C.H.I.P. i2c and GPIO low level and other code to access devices

* CHIP_Gforth_i2c.fs
  * This file makes low level reading and writing to CHIP i2c work.  At this writing I have confirmed on headless OS 4.4 that U14 pin25 and pin26 ( TWI2-SDA(i2c-2-SDA) & TWI2-SCK(i2c-2-SCL) ) work without a device overlay added to the OS.  I talked to a BMP180 device at i2c address #77 with the below gforth software for this sensor.

* BMP180-object.fs
  * This file is an object that can be used to talk to the BMP180 sensor provided it is connected to U14 pin25 and pin26 for i2c-2 connection.  I have confirmed at this writing that this does work with the above CHIP_Gforth_i2c i2c low level words.

* i2c-information.md
  * Information file about CHIP and i2c!
