﻿using AutumnBox.Basic.Devices;
using AutumnBox.Images.DynamicIcons;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace AutumnBox
{
    //除基本的按钮点击逻辑外的UI逻辑
    public partial class Window1
    {
        private Object setUILock = new object();
        public Object rateBoxLock = new object();
        private string _langname = "zh-cn";
        /// <summary>
        /// 更改语言
        /// </summary>
        private void ChangeLanguage()
        {
            if (_langname == "zh-cn")
            {
                Application.Current.Resources.Source = new Uri(@"Lang\en-us.xaml", UriKind.Relative);
                _langname = "en-us";
            }
            else
            {
                Application.Current.Resources.Source = new Uri(@"Lang\zh-cn.xaml", UriKind.Relative);
                _langname = "zh-cn";
            }
        }

        private delegate void NormalEventHandler();
        private event NormalEventHandler SetUIFinish;//设置UI的完成事件,这个事件的处理方法将会关闭进度窗
        /// <summary>
        /// 根据设备改变界面,如果按钮状态,显示文字,这个方法需要用新线程来操作.并且完成后将会发生事件
        /// 通过事件可以便可以关闭进度窗
        /// </summary>
        /// <param name="id">设备的id</param>
        private void SetUIByDevices(object arg)
        {
            string id = arg.ToString();
            this.Dispatcher.Invoke(new Action(() =>
            {
                //根据状态改变按钮状态和设备状态图片
                
                //获取设备信息
                DeviceInfo info = core.GetDeviceInfo(id);
                //根据状态将图片和按钮状态进行设置
                ChangeButtonAndImageByStatus(info.deviceStatus);
                //更改文字
                this.AndroidVersionLabel.Content = info.androidVersion;
                this.CodeLabel.Content = info.code;
                this.ModelLabel.Content = Regex.Replace(info.brand, @"[\r\n]", "") + " " + info.model;
                SetUIFinish?.Invoke();
            }));

        }
        /// <summary>
        /// 根据设备状态改变按钮,图片等的状态
        /// </summary>
        /// <param name="status">设备的状态</param>
        private void ChangeButtonAndImageByStatus(DeviceStatus status)
        {
            bool inBootLoader = false;
            bool inRecovery = false;
            bool inRunning = false;
            bool notFound = false;
            switch (status) {
                case DeviceStatus.FASTBOOT:
                    inBootLoader = true;
                    break;
                case DeviceStatus.RECOVERY:
                    inRecovery = true;
                    break;
                case DeviceStatus.RUNNING:
                    inRunning = true;
                    break;
                default:
                    notFound = true;
                    break;
            }
            this.buttonSideload.IsEnabled = inRecovery;
            this.buttonUnlockMiSystem.IsEnabled = (inRecovery||inRunning);
            this.buttonRelockMi.IsEnabled = inBootLoader;
            this.buttonRebootToBootloader.IsEnabled = !notFound;
            this.buttonRebootToSystem.IsEnabled = !notFound;
            this.buttonRebootToRecovery.IsEnabled = (inRunning || inRecovery);
            this.buttonPushFileToSdcard.IsEnabled = (inRecovery||inRunning);
            this.buttonFlashCustomRecovery.IsEnabled = inBootLoader;
            switch (status)
            {
                case DeviceStatus.FASTBOOT:
                    this.DeviceStatusImage.Source = Tools.BitmapToBitmapImage(DyanamicIcons.fastboot);
                    this.DeviceStatusLabel.Content = FindResource("DeviceInFastboot").ToString();
                    break;
                case DeviceStatus.RECOVERY:
                    this.DeviceStatusImage.Source = Tools.BitmapToBitmapImage(DyanamicIcons.recovery);
                    this.DeviceStatusLabel.Content = FindResource("DeviceInRecovery").ToString();
                    break;
                case DeviceStatus.RUNNING:
                    this.DeviceStatusImage.Source = Tools.BitmapToBitmapImage(DyanamicIcons.poweron);
                    this.DeviceStatusLabel.Content = FindResource("DeviceInRunning").ToString();
                    break;
                default:
                    this.DeviceStatusImage.Source = Tools.BitmapToBitmapImage(DyanamicIcons.no_selected);
                    this.DeviceStatusLabel.Content = FindResource("PleaseSelectedADevice").ToString();
                    break;
            }
        }
    }
}
