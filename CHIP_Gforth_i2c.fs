\ This Gforth code gives i2c functionality to C.H.I.P. hardware in Gforth
\    Copyright (C) 2016  Philip K. Smith

\    This program is free software: you can redistribute it and/or modify
\    it under the terms of the GNU General Public License as published by
\    the Free Software Foundation, either version 3 of the License, or
\    (at your option) any later version.

\    This program is distributed in the hope that it will be useful,
\    but WITHOUT ANY WARRANTY; without even the implied warranty of
\    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\    GNU General Public License for more details.

\    You should have received a copy of the GNU General Public License
\    along with this program.  If not, see <http://www.gnu.org/licenses/>.

\  This software will give access to i2c functions in Gforth but will not take care of Device Tree settings at all.
\  The device tree should be set up before using this software.
\  Note it is possible to access the GPIO stuff without setting device tree stuff but
\  be aware that doing so may cause confusion in linux.
\  This software works with Debian Jessie and may work on other versions.
\  In my research for this i2c code i found this link:
\  http://www.element14.com/community/thread/23991/l/bbb--i2c-notes
\  This code worked well and i was going to do the same stuff so i decided to copy and adapted it here.
\  This authors writing style is like mine and the code all worked!

c-library myCHIPi2c
\c #include <stdio.h>
\c #include <linux/i2c.h>
\c #include <linux/i2c-dev.h>
\c #include <sys/ioctl.h>
\c #include <fcntl.h>
\c #include <errno.h>

\c #define I2C_PASS 0
\c #define I2C_FAIL -1

\c int i2c_open(unsigned char bus, unsigned char addr)
\c {
\c  int file;
\c  char filename[16];
\c  sprintf(filename,"/dev/i2c-%d", bus);
\c  if ((file = open(filename,O_RDWR)) < 0)
\c  {
\c    fprintf(stderr, "i2c_open open error: %s\n", strerror(errno));
\c    return(file);
\c  }
\c  if (ioctl(file,I2C_SLAVE_FORCE,addr) < 0)
\c  {
\c    fprintf(stderr, "i2c_open ioctl error: %s\n", strerror(errno));
\c    return(I2C_FAIL);
\c  }
\c  return(file);
\c }

\c int i2c_write(int handle, unsigned char* buf, unsigned int length)
\c {
\c  if (write(handle, buf, length) != length)
\c  {
\c    fprintf(stderr, "i2c_write error: %s\n", strerror(errno));
\c    return(I2C_FAIL);
\c  }
\c  return(length);
\c }

\c int i2c_write_byte(int handle, unsigned char val)
\c {
\c  if (write(handle, &val, 1) != 1)
\c  {
\c     fprintf(stderr, "i2c_write_byte error: %s\n", strerror(errno));
\c     return(I2C_FAIL);
\c   }
\c   return(I2C_PASS);
\c }

\c int i2c_read(int handle, unsigned char* buf, unsigned int length)
\c {
\c   if (read(handle, buf, length) != length)
\c   {
\c     fprintf(stderr, "i2c_read error: %s\n", strerror(errno));
\c     return(I2C_FAIL);
\c   }
\c   return(length);
\c }

\c int i2c_read_byte(int handle, unsigned char* val)
\c {
\c   if (read(handle, val, 1) != 1)
\c   {
\c     fprintf(stderr, "i2c_read_byte error: %s\n", strerror(errno));
\c     return(I2C_FAIL);
\c   }
\c   return(I2C_PASS);
\c }

\c int i2c_close(int handle)
\c {
\c   if ((close(handle)) != 0)
\c   {
\c     fprintf(stderr, "i2c_close error: %s\n", strerror(errno));
\c     return(I2C_FAIL);
\c   }
\c   return(I2C_PASS);
\c }

\c int i2c_write_read(int handle, unsigned char addr_w, unsigned char *buf_w, unsigned int len_w,
\c                    unsigned char addr_r, unsigned char *buf_r, unsigned int len_r)
\c {
\c 	struct i2c_rdwr_ioctl_data msgset;
\c 	struct i2c_msg msgs[2];
\c
\c 	msgs[0].addr=addr_w;
\c 	msgs[0].len=len_w;
\c 	msgs[0].flags=0;
\c 	msgs[0].buf=buf_w;
\c
\c 	msgs[1].addr=addr_r;
\c 	msgs[1].len=len_r;
\c 	msgs[1].flags=1;
\c 	msgs[1].buf=buf_r;
\c
\c 	msgset.nmsgs=2;
\c 	msgset.msgs=msgs;
\c
\c 	if (ioctl(handle,I2C_RDWR,(unsigned long)&msgset)<0)
\c   {
\c 	fprintf(stderr, "i2c_write_read error: %s\n",strerror(errno));
 \c     return (I2C_FAIL);
\c   }
\c   return(len_r);
\c }

\c int i2c_write_ignore_nack(int handle, unsigned char addr_w, unsigned char* buf, unsigned int length)
\c {
\c 	struct i2c_rdwr_ioctl_data msgset;
\c 	struct i2c_msg msgs[1];
\c
\c 	msgs[0].addr=addr_w;
\c 	msgs[0].len=length;
\c 	msgs[0].flags=0 | I2C_M_IGNORE_NAK;
\c 	msgs[0].buf=buf;
\c
\c 	msgset.nmsgs=1;
\c 	msgset.msgs=msgs;
\c
\c 	if (ioctl(handle,I2C_RDWR,(unsigned long)&msgset)<0)
\c   {
\c 	fprintf(stderr, "i2c_write_ignore_nack error: %s\n",strerror(errno));
 \c     return (I2C_FAIL);
\c   }
\c   return(length);
\c }

\c int i2c_read_no_ack(int handle, unsigned char addr_r, unsigned char* buf, unsigned int length)
\c {
\c 	struct i2c_rdwr_ioctl_data msgset;
\c 	struct i2c_msg msgs[1];
\c
\c 	msgs[0].addr=addr_r;
\c 	msgs[0].len=length;
\c 	msgs[0].flags=I2C_M_RD | I2C_M_NO_RD_ACK;
\c 	msgs[0].buf=buf;
\c
\c 	msgset.nmsgs=1;
\c 	msgset.msgs=msgs;
\c
\c 	if (ioctl(handle,I2C_RDWR,(unsigned long)&msgset)<0)
\c   {
\c 	fprintf(stderr, "i2c_read_no_ack error: %s\n",strerror(errno));
\c      return (I2C_FAIL);
\c   }
\c   return(length);
\c }

c-function CHIPi2copen        i2c_open                        n n -- n
c-function CHIPi2cwrite       i2c_write                     n a n -- n
c-function CHIPi2cwrite-b     i2c_write_byte                  n n -- n
c-function CHIPi2cread        i2c_read                      n a n -- n
c-function CHIPi2cread-b      i2c_read_byte                   n a -- n
c-function CHIPi2cclose       i2c_close                         n -- n
c-function CHIPi2cwriteread   i2c_write_read        n n a n n a n -- n
c-function CHIPwrite-ign-nack i2c_write_ignore_nack       n n a n -- n
c-function CHIPread-no-ack    i2c_read_no_ack             n n a n -- n

end-c-library

    \ All the i2c words return a value.  Some words also get passed a pointer so data can be passed to forth also.
    \ Following is a list of what that return value means per word:
    \ CHIPi2copen        returns -1 or a file handle
    \ CHIPi2cwrite       returns -1 or a length count ( note if lenght count is 0 that means 0 bytes sent)
    \ CHIPi2cwrite-b     returns -1 or 0 this 0 is a pass condition
    \ CHIPi2cread        returns -1 or a length count ( if lenght count is 0 that means 0 bytes read)
    \ CHIPi2cread-b      returns -1 or 0 this 0 is a pass condition
    \ CHIPi2cclose       returns -1 or 0 this 0 is a pass condition
    \ CHIPi2cwriteread   returns -1 or a length count ( if length count is 0 that means 0 bytes read)
    \ CHIPwrite-ign-nack returns -1 or a length count ( if length count is 0 that means 0 bytes sent)
    \ CHIPread-no-ack    returns -1 or a length count ( if length count is 0 that means 0 bytes read)
    \ So the idea here is if a words returns -1 it is a clear protocal failure.  If it is a 0 lentgh count returned
    \ then you can see that in most cases that means something was not sent or recieved but it may not be
    \ and error.  The three words CHIPi2cwrite-b, CHIPi2cread-b and CHIPi2cclose the 0 returned always means a pass for
    \ the function.   This should help in determining what to do based on the returned value!
