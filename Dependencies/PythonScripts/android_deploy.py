#!/usr/bin/env python
from subprocess import Popen, PIPE
from os.path import basename

import binascii
import sys
import re

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
	print ("--obb= an additional binary package")
	print ("--clear when uninstalling old version also clears cache")
	print ("--run to start the unity player activity")
	print ("--run= to start a custom activity")
	print ("")
	pass

def requirements():
	command = ["aapt"]
	p = Popen(command, stdout=PIPE, stderr=PIPE)
	result = p.communicate()
	
	if(result[1].startswith("Android Asset Packaging Tool")):
		print("found AAPT tool.")
		return True
	
	print("")
	print("")
	print("Android deployment helper")
	print("")
	print("")
	print("ERROR: aapt is required. Please add the aapt tool to your path.")
	print("AAPT can be found in e.g. <path_to_your_android_sdk>/sdk/build-tools/android-4.4")

	return False

def isDeviceConnected():
	command = ["adb", "devices"]
	result = call(command)
	return (len(result.splitlines()) > 2)


def call(data):
	#print(data)
	p = Popen(data, stdout=PIPE, stderr=PIPE)
	output, error = p.communicate()
	
	if error is not None and len(error) > 0:
		print("ERROR:" +error.decode("utf-8"))

	return output

def getObbPath():
	if (int(versions[1]) <= 2 and int(versions[0]) == 4) or int(versions[0]) < 4:
		return "/sdcard/Android/obb"
	return "/mnt/shell/emulated/obb"

def uninstall(package,clean):
	print("> Uninstalling old application")
	data = ["adb","shell","pm","uninstall",package]
	if clean:
		print(">> with cleanup")
		data.append("-k")
	call(data)

def getParam(args, param):
	for i in args:
		if i.startswith("--"+param):
			return i[(len(param)+3):]

def getApptParam(apk):
	data = ["aapt", "dump", "badging", apk, "AndroidManifest.xml", "|", "grep", "version"]
	p = Popen(data, stdout=PIPE, stderr=None)
	result = p.communicate()
	apkInfo = result[0].splitlines()
	regex = re.compile("'(.*?)'")
	filtered = regex.findall(apkInfo[0])
	# [packagename, versionCode, versionName]
	return [filtered[0], filtered[1], filtered[2]]

def install(package,apk):
	print("> Installing apk...")
	data = ["adb","install",apk]
	call(data)

def copyobb(package, apk, obbfile, obbpath, vcode):
	print("> Copying obb file...")
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

	manifestInfo = getApptParam(apk)
	packagename = manifestInfo[0]
	vcode = manifestInfo[1]
	vname = manifestInfo[2]
	obbfile = getParam(args,"obb")

	if not run:
		runCustom = getParam(args,"run")

	if apk is None:
		print ("> Please provide an apk file with --apk=...")
		usage()
		return

	if packagename is None:
		print ("> Couldn't read out a packagename.")
		usage()
		return
	
	print("apk: " + apk)
	print("package: " + packagename)
	print("version: " + vcode)
	print("versionName: " + vname)
	if obbfile is not None:
		print("obb: " + obbfile)

	uninstall(packagename,clean)
	install(packagename,apk)
	if obbfile is not None:
		copyobb(packagename, apk,obbfile,obbpath, vcode)

	if run:
		print("> Executing UnityPlayerActivity")
		runActivity(packagename,"com.unity3d.player.UnityPlayerProxyActivity")
	elif runCustom is not None:
		print("> Executing custom activity:" + runCustom)
		runActivity(packagename,runCustom)
	
# -----------------

if not requirements():
	exit()

if not isDeviceConnected():
	print("> Please connect a device.")
	exit()

args = sys.argv[1:]

if "--help" in args or "-h" in args:
	usage()
	exit()
	
output = call(["adb", "shell", "getprop", "ro.build.version.release"])
output = output.decode("utf-8").strip()
versions = output.split(".")

obbpath = ""

main(args)
