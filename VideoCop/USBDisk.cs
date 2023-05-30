using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace VideoCop
{
    public class USBDisk
    {
        public static List<USBDiskInfo> getUSBDiskInfo()
        {
            List<USBDiskInfo> usbDiskInfoList = new List<USBDiskInfo>();
            string diskName = string.Empty;

            foreach (System.Management.ManagementObject drive in
                      new System.Management.ManagementObjectSearcher(
                       "select * from Win32_DiskDrive where InterfaceType='USB'").Get())
            {

                try
                {
                    usbDiskInfoList.Add(parseUSBDiskInfo(drive, diskName));
                }
                catch (Exception)
                {
                    MessageBox.Show("Обнаружена проблема с носителем!\n\n" + drive);
                }
            }

            return usbDiskInfoList;
        }

        private static string parseSerialFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('\\');
            string[] serialArray;
            string serial;
            int arrayLen = splitDeviceId.Length - 1;

            serialArray = splitDeviceId[arrayLen].Split('&');
            serial = serialArray[0];

            return serial;
        }

        private static string parseVenFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('\\');
            string Ven;
            string[] splitVen = splitDeviceId[1].Split('&');

            Ven = splitVen[1].Replace("VEN_", "");
            Ven = Ven.Replace("_", " ");
            return Ven;
        }

        private static string parseProdFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('\\');
            string Prod;
            string[] splitProd = splitDeviceId[1].Split('&');

            Prod = splitProd[2].Replace("PROD_", ""); ;
            Prod = Prod.Replace("_", " ");
            return Prod;
        }

        private static USBDiskInfo parseUSBDiskInfo(System.Management.ManagementObject drive, string diskName)
        {
            USBDiskInfo USBDiskInfo = new USBDiskInfo();

            foreach (System.Management.ManagementObject partition in
            new System.Management.ManagementObjectSearcher(
                "ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" + drive["DeviceID"]
                  + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition").Get())
            {
                foreach (System.Management.ManagementObject disk in
             new System.Management.ManagementObjectSearcher(
                    "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='"
                      + partition["DeviceID"]
                      + "'} WHERE AssocClass = Win32_LogicalDiskToPartition").Get())
                {
                    diskName = disk["Name"].ToString().Trim();
                    USBDiskInfo.DiskLetter = diskName;
                }
            }

            USBDiskInfo.DiskLabel = "";

            var drives = DriveInfo.GetDrives().Where(drive123 => drive123.Name == (diskName + "\\"));

            USBDiskInfo.DiskLabel = drives.FirstOrDefault().VolumeLabel;
            if (USBDiskInfo.DiskLabel.Length == 0)
            {
                USBDiskInfo.DiskLabel = "USB-накопитель";
            }

            USBDiskInfo.Model = (string)drive["Model"];

            USBDiskInfo.Ven = parseVenFromDeviceID(drive["PNPDeviceID"].ToString().Trim());

            USBDiskInfo.Prod = parseProdFromDeviceID(drive["PNPDeviceID"].ToString().Trim());

            USBDiskInfo.Rev = parseRevFromDeviceID(drive["PNPDeviceID"].ToString().Trim());

            string serial = drive["SerialNumber"].ToString().Trim();
            if (serial.Length > 1)
                USBDiskInfo.SerialNumber = serial;
            else
                USBDiskInfo.SerialNumber = parseSerialFromDeviceID(drive["PNPDeviceID"].ToString().Trim());

            decimal dSize = Math.Round((Convert.ToDecimal(
          new System.Management.ManagementObject("Win32_LogicalDisk.DeviceID='"
                  + diskName + "'")["Size"]) / 1073741824), 2);
            USBDiskInfo.DiskSize = dSize + " GB";

            decimal dFree = Math.Round((Convert.ToDecimal(
          new System.Management.ManagementObject("Win32_LogicalDisk.DeviceID='"
                  + diskName + "'")["FreeSpace"]) / 1073741824), 2);
            USBDiskInfo.DiskFree = dFree + " GB";

            decimal dUsed = dSize - dFree;
            USBDiskInfo.DiskUsed = dUsed + " GB";

            return USBDiskInfo;
        }

        private static string parseRevFromDeviceID(string deviceId)
        {
            string[] splitDeviceId = deviceId.Split('\\');
            string Rev;
            string[] splitRev = splitDeviceId[1].Split('&');

            Rev = splitRev[3].Replace("REV_", ""); ;
            Rev = Rev.Replace("_", " ");
            return Rev;
        }
    }

    public class USBDiskInfo
    {
        public USBDiskInfo(string diskLetter, string diskLabel, string model, string ven, string prod, string rev, string serialNumber, string diskSize, string diskFree, string diskUsed)
        {
            this.DiskLetter = diskLetter;
            this.DiskLabel = diskLabel;
            this.Model = model;
            this.Ven = ven;
            this.Prod = prod;
            this.Rev = rev;
            this.SerialNumber = serialNumber;
            this.DiskSize = diskSize;
            this.DiskFree = diskFree;
            this.DiskUsed = diskUsed;
        }

        public USBDiskInfo()
        {
        }

        public string DiskLetter { get; set; }
        public string DiskLabel { get; set; }
        public string Model { get; set; }
        public string Ven { get; set; }
        public string Prod { get; set; }
        public string Rev { get; set; }
        public string SerialNumber { get; set; }
        public string DiskSize { get; set; }
        public string DiskFree { get; set; }
        public string DiskUsed { get; set; }

        public override string ToString()
        {
            return this.DiskLabel + "( " + this.DiskLetter + " )";
        }
    }
}
