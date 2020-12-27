using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FootballFieldManagement.Views;
using FootballFieldManagement.Resources.UserControls;
using System.Windows;
using FootballFieldManagement.Models;
using FootballFieldManagement.DAL;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;

namespace FootballFieldManagement.ViewModels
{
    class TimeFrameViewModel : BaseViewModel
    {
        public ICommand SelectionChangedFieldType { get; set; } //Chọn loại sân
        public ICommand LoadedCommand { get; set; } //Thay đổi thời gian mở của, đóng cửa
        public ICommand SetCommand { get; set; } //Thiết lập khung giờ
        public ICommand SaveCommand { get; set; } //Lưu 
        public ICommand DeleteTimeFrameCommand { get; set; } //Xóa khung giờ
        public ICommand TextChangedCommand { get; set; } //Thay đổi giá của khung giờ
        public ICommand SeparateThousandsCommand { get; set; } //Chuyển sang dạng 0,000,000
        public ICommand AddTimeFrameCommand { get; set; } //Thêm khung giờ mới
        public ICommand ExitCommand { get; set; }
        public ICommand ShowWindowAddTimeFrame { get; set; }
        private ObservableCollection<string> itemSourceFieldType = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceFieldType { get => itemSourceFieldType; set { itemSourceFieldType = value; OnPropertyChanged(); } }

        private bool isChanged; // kiểm tra có sự thay đổi nào không
        public bool IsChanged { get => isChanged;  set => isChanged = value; }
        private string price;
        public string Price { get => price;  set => price = value; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string TimePerMatch { get; set; }

        private SetTimeFrameWindow setTimeWd;
        public SetTimeFrameWindow SetTimeWd { get => setTimeWd; set => setTimeWd = value; }

        public List<TimeFrame> tmpTimeFrames;// Lưu những thứ chưa được lưu dưới DB => khi bấm nút Lưu =>Lưu DB
        public TimeFrameViewModel()
        {
            LoadedCommand = new RelayCommand<SetTimeFrameWindow>(parameter => true, parameter => Load(parameter));

            SelectionChangedFieldType = new RelayCommand<SetTimeFrameWindow>(parameter => true, parameter => ChangedFieldType(parameter));
            SetCommand = new RelayCommand<SetTimeFrameWindow>(parameter => true, parameter => GenerateTimeFrame(parameter));
            DeleteTimeFrameCommand = new RelayCommand<PeriodControl>(parameter => true, parameter => DeleteTimeFrame(parameter));

            SaveCommand = new RelayCommand<SetTimeFrameWindow>(parameter => true, parameter => SaveData(parameter));
            ExitCommand = new RelayCommand<SetTimeFrameWindow>(parameter => true, parameter => parameter.Close());

            TextChangedCommand = new RelayCommand<PeriodControl>(parameter => true, parameter => TextChanged(parameter));
            SeparateThousandsCommand = new RelayCommand<TextBox>(parameter => true, parameter => SeparateThousands(parameter));

            ShowWindowAddTimeFrame = new RelayCommand<SetTimeFrameWindow>(parameter => true, parameter => ShowAddTimeFrame(parameter));
            AddTimeFrameCommand = new RelayCommand<AddTimeFrameWindow>(parameter => true, parameter => AddTimeFrame(parameter));
        }
        public void CloseWindow(object sender, CancelEventArgs e)
        {
            TimeFrame time = tmpTimeFrames.Find(x => x.Price == -1);
            if (isChanged ||  time != null)
            {
                MessageBoxResult result = CustomMessageBox.Show("Bạn có muốn lưu hay không?", "Thông báo", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    if (IsPriceNull(SetTimeWd))
                    {
                        e.Cancel = true;
                        return;
                    }
                    SaveData(SetTimeWd);
                    if (!isChanged )
                    {
                        e.Cancel = false;
                    }
                }
                else if(result == MessageBoxResult.No)
                {
                    if (TimeFrameDAL.Instance.CheckPriceIsNull())
                    {
                        e.Cancel = true;
                        CustomMessageBox.Show("Khung giờ của loại sân mới chưa được thiết lập!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
        public void Load(SetTimeFrameWindow wdSetTime)
        {
            this.IsChanged = false;
            this.setTimeWd = wdSetTime;
            setItemSourceFieldType();
            tmpTimeFrames = TimeFrameDAL.Instance.ConvertDBToList();
            wdSetTime.cboFieldType.SelectedIndex = 0;
            if (tmpTimeFrames.Count != 0)
            {
                wdSetTime.tpkOpenTime.SelectedTime = DateTime.Parse(tmpTimeFrames[0].StartTime);
                wdSetTime.tpkCloseTime.SelectedTime = DateTime.Parse(tmpTimeFrames[tmpTimeFrames.Count - 1].EndTime);
            }
            wdSetTime.cboTimePerMatch.Text = null;
            ChangedFieldType(wdSetTime);
        }
        public void GenerateTimeFrame(SetTimeFrameWindow wdSetTime)
        {
            if (string.IsNullOrEmpty(wdSetTime.tpkOpenTime.Text))
            {
                wdSetTime.tpkOpenTime.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdSetTime.tpkCloseTime.Text))
            {
                wdSetTime.tpkCloseTime.Focus();
                return;
            }
            if (string.IsNullOrEmpty(wdSetTime.cboTimePerMatch.Text))
            {
                wdSetTime.cboTimePerMatch.Focus();
                wdSetTime.cboTimePerMatch.Text = "";
                return;
            }
            if (CovertToMinute(wdSetTime.tpkOpenTime.Text) > CovertToMinute(wdSetTime.tpkCloseTime.Text))
            {
                wdSetTime.cboTimePerMatch.SelectedItem = null;
                wdSetTime.tpkOpenTime.Text = null;
                wdSetTime.tpkCloseTime.Text = null;
                CustomMessageBox.Show("Giờ bắt đầu nhỏ hơn giờ kết thúc!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            this.tmpTimeFrames.Clear();
            wdSetTime.stkTime.Children.Clear();
            this.isChanged = true;
            //Chia khung giờ
            int openTime = CovertToMinute(wdSetTime.tpkOpenTime.Text);
            int closeTime = CovertToMinute(wdSetTime.tpkCloseTime.Text);
            int step;
            step = 30 * (wdSetTime.cboTimePerMatch.SelectedIndex + 2);
            for (int i = openTime; i <= closeTime - step; i += step)
            {
                string str1 = (i % 60).ToString();
                if (i % 60 < 10)
                {
                    str1 = "0" + str1;
                }
                string str2 = ((i + step) % 60).ToString();
                if ((i + step) % 60 < 10)
                {
                    str2 = "0" + str2;
                }
                string strStartTime = ((i / 60 < 10) ? "0" : "") + (i / 60).ToString() + ":" + str1;
                string strEndTime = (((i + step) / 60) < 10 ? "0" : "") + ((i + step) / 60).ToString() + ":" + str2;
                PeriodControl control = new PeriodControl();
                control.txtStartTime.Text = strStartTime;
                control.txtEndTime.Text = strEndTime;
                control.txbId.Text = (tmpTimeFrames.Count + 1).ToString();
                wdSetTime.stkTime.Children.Add(control);

                //Add vào 1 list tạm => mục đích để hiển thị thông tin đang sửa(chưa lưu xuống DB)
                foreach (string item in FootballFieldDAL.Instance.GetFieldType())
                {
                    TimeFrame time = new TimeFrame(tmpTimeFrames.Count + 1, strStartTime, strEndTime, int.Parse(item), -1);
                    this.tmpTimeFrames.Add(time);
                }
            }
        }
        public void ChangedFieldType(SetTimeFrameWindow wdSetTime)
        {
            if (wdSetTime.cboFieldType.SelectedItem == null)
                return;
            wdSetTime.stkTime.Children.Clear();
            string tmp = wdSetTime.cboFieldType.SelectedValue.ToString();
            string fieldType = tmp.Split(' ')[1];
            foreach (TimeFrame time in this.tmpTimeFrames.FindAll(x => x.FieldType.ToString() == fieldType))
            {
                PeriodControl control = new PeriodControl();
                control.txbId.Text = time.Id.ToString();
                control.txtStartTime.Text = time.StartTime;
                control.txtEndTime.Text = time.EndTime;
                if (time.Price != -1)
                {
                    control.txtPrice.Text = string.Format("{0:N0}", time.Price);
                }
                control.txtPrice.SelectionStart = control.txtPrice.Text.Length;
                control.txtPrice.SelectionLength = 0;
                wdSetTime.stkTime.Children.Add(control);
            }
        }
        public void TextChanged(PeriodControl control)
        {
            TimeFrame tmp = this.tmpTimeFrames.Find(x => x.Id.ToString() == control.txbId.Text);
            if (string.IsNullOrEmpty(control.txtPrice.Text))
            {
                tmp.Price = -1;
            }
            else
            {
                tmp.Price = ConvertToNumber(control.txtPrice.Text);
            }
            this.isChanged = true;
        }
        public void DeleteTimeFrame(PeriodControl control)
        {
            MessageBoxResult result = CustomMessageBox.Show("Sẽ xóa những loại sân có cùng khung giờ này. Bạn có muốn xóa?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                this.tmpTimeFrames.RemoveAll(x => x.StartTime == control.txtStartTime.Text && x.EndTime == control.txtEndTime.Text);
                this.setTimeWd.stkTime.Children.Remove(control);
                if (tmpTimeFrames.Count != 0)
                {
                    setTimeWd.tpkOpenTime.Text = tmpTimeFrames[0].StartTime;
                    setTimeWd.tpkCloseTime.Text = tmpTimeFrames[tmpTimeFrames.Count - 1].EndTime;
                }
                this.isChanged = true;
            }
        } 

        //Kiểm tra giá của các khung giờ có null hay không
        public bool IsPriceNull(SetTimeFrameWindow wdSetTime)
        {
            var listTemp = tmpTimeFrames.FindAll(x => x.Price == -1).OrderBy(x => x.FieldType).ToList();
            if (listTemp.Count() != 0)
            {
                int i = 0;
                if (listTemp[0].FieldType.ToString() == wdSetTime.cboFieldType.Text.Split(' ')[1])
                {
                    CustomMessageBox.Show("Vui lòng nhập giá!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                foreach (string fieldType in FootballFieldDAL.Instance.GetFieldType())
                {
                    if (listTemp[0].FieldType.ToString() == fieldType)
                    {
                        wdSetTime.cboFieldType.SelectedIndex = i;
                        return true;
                    }
                    i++;
                }
            }
            return false;
        }
        public void SaveData(SetTimeFrameWindow wdSetTime)
        {
            //Lưu
            if (!IsPriceNull(wdSetTime) && isChanged)
            {
                TimeFrameDAL.Instance.ClearData();
                bool isSuccess = true;
                for (int i = 0; i < tmpTimeFrames.Count; i++)
                {
                    TimeFrame newTime = tmpTimeFrames[i];
                    if (!TimeFrameDAL.Instance.AddTimeFrame(newTime))
                    {
                        isSuccess = false;
                        break;
                    }
                }
                if (isSuccess)
                {
                    CustomMessageBox.Show("Đã lưu thành công!", "Thông báo", MessageBoxButton.OK);
                    isChanged = false;
                    wdSetTime.Close();
                }
                else
                {
                    CustomMessageBox.Show("Lưu lỗi!", "Thông báo", MessageBoxButton.OK);
                }
            }
        }
        public void setItemSourceFieldType()
        {
            this.itemSourceFieldType.Clear();
            foreach (var fieldType in FootballFieldDAL.Instance.GetFieldType())
            {
                itemSourceFieldType.Add("Sân " + fieldType + " người");
            }
        }
        public void ShowAddTimeFrame(SetTimeFrameWindow wdSetTime)
        {
            AddTimeFrameWindow newWindow = new AddTimeFrameWindow();
            newWindow.ShowDialog();
        }
        public void AddTimeFrame(AddTimeFrameWindow wdAddTime)
        {
            if (string.IsNullOrEmpty(wdAddTime.tpkStartTime.Text) || string.IsNullOrEmpty(wdAddTime.tpkEndTime.Text) || string.IsNullOrEmpty(wdAddTime.txtPrice.Text))
            {
                CustomMessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //Kiểm tra khung giờ thêm vào có hợp lệ hay không
            bool isSuccess = true;
            foreach (TimeFrame item in tmpTimeFrames.FindAll(x => x.FieldType == 5))
            {
                if (CovertToMinute(item.StartTime) < CovertToMinute(wdAddTime.tpkStartTime.Text) && CovertToMinute(item.EndTime) > CovertToMinute(wdAddTime.tpkStartTime.Text))
                {
                    isSuccess = false;
                }
                if (CovertToMinute(item.StartTime) < CovertToMinute(wdAddTime.tpkEndTime.Text) && CovertToMinute(item.EndTime) > CovertToMinute(wdAddTime.tpkEndTime.Text))
                {
                    isSuccess = false;
                }
                if (CovertToMinute(item.StartTime) == CovertToMinute(wdAddTime.tpkStartTime.Text) || CovertToMinute(item.EndTime) == CovertToMinute(wdAddTime.tpkEndTime.Text))
                {
                    isSuccess = false;
                }
            }
            if (CovertToMinute(wdAddTime.tpkStartTime.Text) > CovertToMinute(wdAddTime.tpkEndTime.Text))
            {
                CustomMessageBox.Show("Khung giờ không hợp lệ!", "Thông báo", MessageBoxButton.OK);
                wdAddTime.tpkEndTime.Text = null;
                wdAddTime.tpkStartTime.Text = null;
                wdAddTime.txtPrice.Text = null;
                return;
            }
            if (isSuccess)
            {
                PeriodControl control = new PeriodControl();
                string str1 = wdAddTime.tpkStartTime.Text.Split(':')[0];
                string str2 = wdAddTime.tpkEndTime.Text.Split(':')[0];
                control.txtStartTime.Text = (int.Parse(str1) < 10 ? "0" : "") + wdAddTime.tpkStartTime.Text;
                control.txtEndTime.Text = (int.Parse(str2) < 10 ? "0" : "") + wdAddTime.tpkEndTime.Text;
                control.txtPrice.Text = wdAddTime.txtPrice.Text;
                foreach (string fieldType in FootballFieldDAL.Instance.GetFieldType())
                {
                    long price = ConvertToNumber(control.txtPrice.Text);
                    if(fieldType != setTimeWd.cboFieldType.Text.Split(' ')[1])
                    {
                        price = -1;
                    }
                    TimeFrame newTime = new TimeFrame(tmpTimeFrames[tmpTimeFrames.Count - 1].Id + 1, control.txtStartTime.Text, control.txtEndTime.Text, int.Parse(fieldType), price);
                    tmpTimeFrames.Add(newTime);
                }
                tmpTimeFrames = tmpTimeFrames.OrderBy(x => x.StartTime).ToList();
                setTimeWd.tpkOpenTime.Text = tmpTimeFrames[0].StartTime;
                setTimeWd.tpkCloseTime.Text = tmpTimeFrames[tmpTimeFrames.Count - 1].EndTime;
                ChangedFieldType(setTimeWd);
                isChanged = true;
                wdAddTime.Close();
            }
            else
            {
                CustomMessageBox.Show("Khung giờ đã bị trùng lặp!", "Thông báo", MessageBoxButton.OK);
                wdAddTime.tpkEndTime.Text = null;
                wdAddTime.tpkStartTime.Text = null;
                wdAddTime.txtPrice.Text = null;
            }
        }
        public int CovertToMinute(string time)
        {
            string[] tmp = time.Split(':');
            return int.Parse(tmp[0]) * 60 + int.Parse(tmp[1]);
        }
    }
}
