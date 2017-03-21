import sys
from struct import *


def read_int4_be(f):
    return unpack('>i', f.read(4))[0]

class Cookie:

    def __init__(self, data):
        self.cookieData = data
        self.analysizeCookie()

    def analysizeCookie(self):
        self.cookieSize = unpack('<i', self.cookieData[:4])[0]
        self.offsetURL = unpack('<i', self.cookieData[16:20])[0]
        self.offsetName = unpack('<i', self.cookieData[20:24])[0]
        self.offsetPath = unpack('<i', self.cookieData[24:28])[0]
        self.offsetValue = unpack('<i', self.cookieData[28:32])[0]
        self.expirationDate = unpack('d', self.cookieData[40:48])[0]
        self.createdDate = unpack('d', self.cookieData[48:56])[0]

        idx = self.offsetName+1
        self.name = self.readAsciiString(idx)

        idx = self.offsetValue+1
        self.value = self.readAsciiString(idx)

        idx = self.offsetURL+1
        self.url = self.readAsciiString(idx)

        idx = self.offsetPath+1
        self.path = self.readAsciiString(idx)

    def readAsciiString(self, idx):
        str = ""
        while True:
            if idx == self.cookieSize:
                break
            temp = chr(self.cookieData[idx])
            str = str + temp
            if self.cookieData[idx] == 0:
                break
            idx += 1
        return str

    def writeCookieData(self, fw):
        fw.write("=======================================\n")
        fw.write("name  : " + self.name  + "\n")
        fw.write("value : " + self.value + "\n")
        fw.write("url   : " + self.url   + "\n")
        fw.write("path  : " + self.path  + "\n")


class Page:
    def __init__(self, f, _startPoint, pageSize):
        
        self.startPoint = _startPoint
         
        cookieHeader = f.read(4) # Page Header

        self.cookieCount = unpack('<i', f.read(4))[0]

        self.cookieOffset = []
        for i in range(self.cookieCount):
            self.cookieOffset.append(unpack('<i', f.read(4))[0])

        f.read(4) # Last CookieOffset
        
        self.cookies = []
        for i in range(self.cookieCount-1):
            f.seek(self.startPoint + self.cookieOffset[i])
            self.cookies.append(Cookie(f.read(self.cookieOffset[i+1]-self.cookieOffset[i])))

        self.cookies.append(Cookie(f.read(pageSize-self.cookieOffset[self.cookieCount-1])))

    def getCookiesNum(self) :
        return self.cookieCount

    def writeCookies(self, fw):
        for i in range(self.cookieCount):
            self.cookies[i].writeCookieData(fw)



f = open('Cookies.binarycookies','rb')
s = f.read(4)

pageNum = int.from_bytes(f.read(4), byteorder='big')


pageSize = []

for i in range(pageNum):
    pageSize.append(unpack('>i', f.read(4))[0])

Pages = []
startPoint = f.tell()
for i in range(pageNum):
    Pages.append(Page(f, startPoint, pageSize[i]))
    startPoint += pageSize[i]

fw = open('Cookie data.txt','w')
for i in range(pageNum):
    Pages[i].writeCookies(fw)

f.close()
fw.close()

print("Bye!")
