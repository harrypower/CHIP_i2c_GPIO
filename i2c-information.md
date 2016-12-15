# I2C information on CHIP

Notes on using i2c on CHIP:

`sudo i2cdetect -l`

This will show you a list of i2c devices on your CHIP like the following:
```
i2c-0   i2c             mv64xxx_i2c adapter                     I2C adapter
i2c-1   i2c             mv64xxx_i2c adapter                     I2C adapter
i2c-2   i2c             mv64xxx_i2c adapter                     I2C adapter
```

The i2c devices get enumerated but at this moment i could not find any software user manual to refer to for the meaning of the addresses to the pins.  The following command will give this information but the references will need to come later!

`sudo ls -l /sys/bus/i2c/devices/i2c-*`

This will produce :

```
lrwxrwxrwx 1 root root 0 Dec 12 00:33 /sys/bus/i2c/devices/i2c-0 -> ../../../devices/platform/soc@01c00000/1c2ac00.i2c/i2c-0
lrwxrwxrwx 1 root root 0 Dec 12 00:33 /sys/bus/i2c/devices/i2c-1 -> ../../../devices/platform/soc@01c00000/1c2b000.i2c/i2c-1
lrwxrwxrwx 1 root root 0 Dec 12 00:33 /sys/bus/i2c/devices/i2c-2 -> ../../../devices/platform/soc@01c00000/1c2b400.i2c/i2c-2
```

Notice the address of i2c-0, i2c-1 and i2c-2 are 1c2ac00 and 1c2b000 and 1c2b400. All i know at this moment is the i2c-2 is available on headless OS 4.4 at pins U14 pin25 and pin26. I will get a schematic and a technical manual sometime to learn more information.

You can get the CHIP to tell you the devices address as follows:
`i2cdetect -r 2`

This is going to show the most common addresses connected to i2c-2 from linux point of view but i have no idea from from the SOC's point of view. You should see something like the following:
```
WARNING! This program can confuse your I2C bus, cause data loss and worse!
I will probe file /dev/i2c-2 using read byte commands.
I will probe address range 0x03-0x77.
Continue? [Y/n] y
     0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f
00:          -- -- -- -- -- -- -- -- -- -- -- -- --
10: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
20: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
30: -- -- -- -- -- -- -- -- UU -- -- -- -- -- -- --
40: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
50: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
60: -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- --
70: -- -- -- -- -- -- -- 77
```

Notice the warning! It is real because some devices can be reset or locked when they are accessed this way!
This example shows a device at address 0x77 and that device is in my case a BMP180 pressure temperature sensor! The UU means those addresses were skipped because linux has some devices in use at those addresses.
