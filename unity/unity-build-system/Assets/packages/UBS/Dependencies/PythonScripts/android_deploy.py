#!/usr/bin/env python
from subprocess import Popen, PIPE
from os.path import basename

import binascii
import sys

# android_deploy.py --apk=UBS3-Android-Public/carcassonne.apk --packagename=com.exozet.game.carcassonne --vcode=5187 --obb=UBS3-Android-Public/carcassonne.main.obb
# --clear clears all cache and save game data, optional
# --obb is optional
# --run is optional


def usage():
	print("")
	print("")
	print("Android deployment helper")
	print("")
	print("")
	print ("EXAMPLE: ./android_deploy.py --apk=UBS3-Android-Public/carcassonne.apk --packagename=com.exozet.game.carcassonne --vcode=5187 --obb=UBS3-Android-Public/carcassonne.main.obb")
	print("")
	print("")
	print("Copy the given apk file onto the attached android device. Previous installations will be removed. Copies obb file to to the device as well. ")
	print("")
	print("")
	print ("--help prints this help")
	print ("--apk= the apk file to use")
	print ("--packagename= the package name of the application")
	print ("--vcode= the version code of the apk")
	print ("--obb= an additional binary package")
	print ("--clear when uninstalling old version also clears cache")
	print ("--run to start the unity player activity")
	print ("--run= to start a custom activity")
	print ("")
	pass

def call(data):
	print( data)
	p = Popen(data, stdout=PIPE, stderr=PIPE)
	output, error = p.communicate()
	
	if error is not None and len(error) > 0:
		print("ERROR:" +error.decode("utf-8"))

	return output

def getObbPath():
	if int(versions[0]) <= 4:
		if int(versions[1]) <= 2:
			return "/sdcard/Android/obb/"
	return "/mnt/shell/emulated/obb"

def uninstall(package,clean):
	data = ["adb","shell","pm","uninstall",package]
	if clean:
		data.append("-k")
	call(data)

def getParam(args, param):
	for i in args:
		if i.startswith("--"+param):
			return i[(len(param)+3):]

def install(package,apk):
	data = ["adb","install",apk]
	call(data)

def copyobb(package, apk, obbfile, obbpath, vcode):
	data = ["adb","shell","mkdir","-p",obbpath+package]
	call(data)
	
	data = ["adb", "push", obbfile, obbpath+"/"+package+"/"+"main."+vcode+"."+package+".obb"]
	call(data)

def runActivity(package,activity):
	data = ["adb","shell", "am", "start", "-n", package+"/"+activity]
	call(data)

def main(args):
	

	obbpath = getObbPath()

	clean = '--clear' in args
	run = '--run' in args
	apk = getParam(args,"apk")
	
	packagename = getParam(args,"packagename")
	obbfile = getParam(args,"obb")
	vcode = getParam(args,"vcode")

	if not run:
		runCustom = getParam(args,"run")

	if apk is None:
		print ("no apk given")
		usage()
		return

	if packagename is None:
		print ("no package given")
		usage()
		return
	
	print("apk: " + apk)
	print("package: " + packagename)
	if obbfile is not None:
		print("obb: " + obbfile)

	uninstall(packagename,clean)
	install(packagename,apk)
	if obbfile is not None:
		copyobb(packagename, apk,obbfile,obbpath, vcode)

	if run:
		runActivity(packagename,"com.unity3d.player.UnityPlayerProxyActivity")
	elif runCustom is not None:
		runActivity(packagename,runCustom)
	
args = sys.argv[1:]

if "--help" in args or "-h" in args:
	usage()
	exit()
	

output = call(["adb", "shell", "getprop", "ro.build.version.release"])
output = output.decode("utf-8").strip()
versions = output.split(".")


obbpath = ""

main(args)
