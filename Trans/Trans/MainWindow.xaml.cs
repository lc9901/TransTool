﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Trans.Business;
using Trans.Entity;
using Trans.Utils;

namespace Trans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , IDisposable
    {
        #region privare method
        private XmlHelper xmlHelper = new XmlHelper();
        private TransController transController = new TransController();
        private BackgroundWorker back = new BackgroundWorker();
        #endregion

        #region Construct method

        /// <summary>
        /// Improve the instance.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.InitContext();
            this.InitEvent();
        }

        #endregion

        #region init method

        /// <summary>
        /// Initialize the context for the combox.
        /// </summary>
        private void InitContext()
        {
            Collection<DropListItem> list = xmlHelper.GetDropList();
            ComboBoxItem item;

            foreach (var i in list)
            {
                item = new ComboBoxItem
                {
                    Tag = i,
                    Content = i.DisplayName
                };

                this.comBoxInputType.Items.Add(item);
                
                if ("zh".Equals(i.Code))
                {
                    item.IsSelected = true;
                }

            }

            foreach (var i in list)
            {
                item = new ComboBoxItem
                {
                    Tag = i,
                    Content = i.DisplayName
                };

                this.comBoxOutPut.Items.Add(item);

                if ("auto".Equals(i.Code))
                {
                    item.IsSelected = true;
                }

            }
        }

        /// <summary>
        /// Initializes the event.
        /// </summary>
        private void InitEvent()
        {
            this.btnExchange.Click += DoBtnExchangeClick;
            this.btnTrans.Click += DoBtnTransClick;
            this.back.DoWork += DoBack;
            this.back.RunWorkerCompleted += DoWorkComplete;
            this.btnCopy.Click += DoBtnCopyClick;
            this.KeyDown += DoWinKeyDown;
            this.txtBoxInput.TextChanged += DoTextChanged; ;
        }

        #endregion

        #region event method
        /// <summary>
        /// Handles the context change event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!back.IsBusy)
            {
                TriggerActive();
            }
        }

        /// <summary>
        /// Handles the key down event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWinKeyDown(object sender, KeyEventArgs e)
        {
            if (this.Activate() && "return".ToLower().Equals(e.Key.ToString().ToLower()) && !back.IsBusy)
            {
                TriggerActive();
            }
        }

        /// <summary>
        /// Copys the result to the clip board.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoBtnCopyClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(this.txtBoxOutPut.Text);
        }

        /// <summary>
        /// Updatas the content of the output text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Cancelled)
            {
                lblStatus.Background = new SolidColorBrush(Colors.Red);
            }
            else
            {
                lblStatus.Background = new SolidColorBrush(Colors.Green);
                txtBoxOutPut.Text = e.Result as string;
            }


        }

        /// <summary>
        /// Execute the logic for the trans.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoBack(object sender, DoWorkEventArgs e)
        {
            string[] args = e.Argument as string[];
            try
            {
                e.Result = transController.GetTargetTesult(args[0], args[1], args[2]);
            }
            catch (Exception)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Async execute the task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoBtnTransClick(object sender, RoutedEventArgs e)
        {
            if (!back.IsBusy)
            {
                TriggerActive();
            }
        }

        /// <summary>
        /// Exchange the selected item between input type and output type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoBtnExchangeClick(object sender, RoutedEventArgs e)
        {
            string type1 = (((ComboBoxItem)comBoxInputType.SelectedItem).Tag as DropListItem).DisplayName;
            string type2 = (((ComboBoxItem)comBoxOutPut.SelectedItem).Tag as DropListItem).DisplayName;

            foreach (var item in comBoxInputType.Items)
            {
                ComboBoxItem i = item as ComboBoxItem;
                if (null != i && i.Content.Equals(type2))
                {
                    i.IsSelected = true;
                }
            }

            foreach (var item in comBoxOutPut.Items)
            {
                ComboBoxItem i = item as ComboBoxItem;
                if (null != i && i.Content.Equals(type1))
                {
                    i.IsSelected = true;
                }
            }

        }

        #endregion

        #region Trigger

        /// <summary>
        /// Trigger of the translate.
        /// </summary>
        private void TriggerActive()
        {
            DropListItem selectedInputType = ((ComboBoxItem)comBoxInputType.SelectedItem).Tag as DropListItem;
            DropListItem selectTargetType = ((ComboBoxItem)comBoxOutPut.SelectedItem).Tag as DropListItem;
            if (null != selectedInputType && null != selectTargetType && !string.IsNullOrEmpty(txtBoxInput.Text.Trim()))
            {
                string[] args = {
                    txtBoxInput.Text,
                    selectedInputType.Code,
                    selectTargetType.Code
                };
                back.RunWorkerAsync(args);
            }
        }

        #endregion

        #region Dispose

        ~MainWindow()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.btnExchange.Click -= DoBtnExchangeClick;
                this.btnTrans.Click -= DoBtnTransClick;
                this.back.DoWork -= DoBack;
                this.back.RunWorkerCompleted -= DoWorkComplete;
                this.btnCopy.Click -= DoBtnCopyClick;
                this.KeyDown -= DoWinKeyDown;
                this.txtBoxInput.TextChanged -= DoTextChanged;
                back.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}