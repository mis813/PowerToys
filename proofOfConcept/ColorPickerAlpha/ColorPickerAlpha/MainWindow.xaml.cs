﻿using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorPickerAlpha
{
    public partial class MainWindow : Window
    {
        Boolean rgbState = false;
        Color curColor;
        public bool isClicked = true; //automatically start looking for colors
        public bool isInWindow = false;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += delegate
            {
                MouseLeave += delegate { isInWindow = false; };
                MouseEnter += delegate { isInWindow = true; };
                Activated += delegate { Topmost = true; };
                Deactivated += delegate { Topmost = true; };

                var hiddenMouseWindow = new OverlayWindow(this);
                hiddenMouseWindow.Show();
            };

            new Thread(() =>
            {
                while (true)
                {
                    if (!isClicked || isInWindow)
                        continue;

                    (int x, int y) = ColorPicker.GetPhysicalCursorCoords();

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        System.Drawing.Color color = ColorPicker.GetPixelColor(x, y);

                        curColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
                        Color_Box.Fill = new SolidColorBrush(curColor);

                        R_val.Text = curColor.R.ToString();
                        G_val.Text = curColor.G.ToString();
                        B_val.Text = curColor.B.ToString();

                        HEXValue.Text = argbToHEX(curColor.ToString());

                    }));

                    Thread.Sleep(100);
                }
            }).Start();
        }

        private void Toggle_RGB(object sender, RoutedEventArgs e)
        {
            rgbState = !rgbState;

            var rgbVisibility = rgbState ? Visibility.Visible : Visibility.Hidden;
            var hexVisibility = !rgbState ? Visibility.Visible : Visibility.Hidden;

            R_val.Visibility = rgbVisibility;
            G_val.Visibility = rgbVisibility;
            B_val.Visibility = rgbVisibility;

            RLabel.Visibility = rgbVisibility;
            GLabel.Visibility = rgbVisibility;
            BLabel.Visibility = rgbVisibility;

            HEXValue.Visibility = hexVisibility;
            HEXLabel.Visibility = hexVisibility;

        }

        private void Copy_Clip(object sender, RoutedEventArgs e) => CopyToClipboard(); 

        public void CopyToClipboard()
        {
            string argb = curColor.ToString();

            if (rgbState)
            {
                string rgbText = "(" + R_val.Text + ", " + G_val.Text + ", " + B_val.Text + ")";
                Clipboard.SetText(rgbText);
            }
            else
            {
                Clipboard.SetText(argbToHEX(argb));
            }
        }

        private string argbToHEX(string argb)
        {
            // RGB and ARGB formats
            StringBuilder hex = new StringBuilder();
            // Append the # sign in hex and remove the Alpha values from the ARGB format i.e) #AARRGGBB.
            hex.Append(argb[0]);
            hex.Append(argb.Substring(3));

            return hex.ToString();
        }

        private void Eyedropper_Click(object sender, RoutedEventArgs e)
        {
            isClicked = !isClicked;
        }
    }
}
