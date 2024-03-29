﻿using FootballFieldManagement.Views;
using FootballFieldManagement.Models;
using FootballFieldManagement.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FootballFieldManagement.Resources.UserControls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace FootballFieldManagement.ViewModels
{
    class FootballFieldViewModel : BaseViewModel
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        //FieldDetailsControl
        public ICommand DeleteListFieldCommand { get; set; }
        public ICommand EditListFieldCommand { get; set; }
        //AddFootballFieldWindow
        public ICommand SaveCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand SeparateThousandsCommand { get; set; }
        public ICommand SelectionFieldTypeCommand { get; set; } // chọn loại sân trong window add field
        public ICommand LostFocusCommand { get; set; } // Dùng để thêm 1 loại sân mới
        //Field Control
        public ICommand HoverCommand { get; set; }
        public ICommand LeaveCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand EditCardFieldCommand { get; set; }
        public ICommand DeleteCardFieldCommand { get; set; }

        //Home Window
        public ICommand SelectionChangedCommand { get; set; } // thay đổi view
        public ICommand AddFieldCommand { get; set; }
        public ICommand SetTimeFrameCommand { get; set; }
        public ICommand OpenReportFieldCommand { get; set; }
        // ReportFieldWindow
        public ICommand ReportFieldCommand { get; set; }
        public ICommand LoadFieldCommand { get; set; }
        private ObservableCollection<FootballField> itemSourceFieldName = new ObservableCollection<FootballField>();
        public ObservableCollection<FootballField> ItemSourceFieldName { get => itemSourceFieldName; set { itemSourceFieldName = value; OnPropertyChanged(); } }

        public FootballField SelectedField { get => selectedField; set { selectedField = value; OnPropertyChanged("SelectedField"); } }

        private FootballField selectedField = new FootballField();
        private HomeWindow home;
        public HomeWindow Home { get => home; set => home = value; }
        private ObservableCollection<string> itemSourceField = new ObservableCollection<string>();
        public ObservableCollection<string> ItemSourceField { get => itemSourceField; set { itemSourceField = value; OnPropertyChanged(); } }

        private FieldDetailsControl detailsField;
        public FieldDetailsControl DetailsField { get => detailsField; set => detailsField = value; }

        private FieldControl cardField;
        public FieldControl CardField { get => cardField; set => cardField = value; }

        public FootballFieldViewModel()
        {
            EditListFieldCommand = new RelayCommand<FieldDetailsControl>(parameter => true, parameter => ClickEditListField(parameter));
            EditCardFieldCommand = new RelayCommand<FieldControl>(parameter => true, parameter => ClickEditCardField(parameter));
            DeleteCardFieldCommand = new RelayCommand<FieldControl>(parameter => true, parameter => DeleteCardField(parameter));
            DeleteListFieldCommand = new RelayCommand<FieldDetailsControl>(parameter => true, parameter => DeleteListField(parameter));
            SaveCommand = new RelayCommand<AddFootballFieldWindow>(parameter => true, parameter => SaveField(parameter));
            ExitCommand = new RelayCommand<AddFootballFieldWindow>(parameter => true, parameter => parameter.Close());
            SeparateThousandsCommand = new RelayCommand<TextBox>(parameter => true, parameter => SeparateThousands(parameter));
            HoverCommand = new RelayCommand<FieldControl>((parameter) => true, (parameter) => Hover(parameter));
            LeaveCommand = new RelayCommand<FieldControl>((parameter) => true, (parameter) => Leave(parameter));
            LoadedCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => LoadField(parameter));
            SelectionChangedCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => SelectionChanged(parameter));
            AddFieldCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => ShowAddField(parameter));
            SetTimeFrameCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => ShowWdSetTimeFrame());
            LostFocusCommand = new RelayCommand<AddFootballFieldWindow>((parameter) => true, (parameter) => LostFocusComboBox(parameter));
            OpenReportFieldCommand = new RelayCommand<object>((parameter) => true, (parameter) => OpenReportFieldWindow());
            LoadFieldCommand = new RelayCommand<object>((parameter) => true, (parameter) => LoadFieldName());
            ReportFieldCommand = new RelayCommand<Window>((parameter) => true, (parameter) => ReportField(parameter));
        }
        public void ReportField(Window window)
        {
            FootballField footballField = selectedField;
            footballField.Status = 0;
            if (FootballFieldDAL.Instance.UpdateField(footballField))
            {
                CustomMessageBox.Show("Báo lỗi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                window.Close();
            }
        }
        public void LoadFieldName()
        {
            itemSourceFieldName.Clear();
            foreach (var field in FootballFieldDAL.Instance.GetGoodFields())
            {
                itemSourceFieldName.Add(field);
            }
        }
        public void OpenReportFieldWindow()
        {
            ReportFieldWindow reportFieldWindow = new ReportFieldWindow();
            reportFieldWindow.ShowDialog();
        }
        public void LostFocusComboBox(AddFootballFieldWindow addFieldWindow)
        {
            string str = addFieldWindow.cboFieldType.Text;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }
            if (str[0] >= 48 && str[0] <= 57)
            {
                addFieldWindow.cboFieldType.Text = "Sân " + str + " người";
            }
        }
        public void setItemSourceFieldName()
        {
            itemSourceFieldName.Clear();
            foreach (var field in FootballFieldDAL.Instance.GetGoodFields())
            {
                itemSourceFieldName.Add(field);
            }
        }
        public void setItemSourceField()
        {
            itemSourceField.Clear();
            foreach (string fieldType in FootballFieldDAL.Instance.GetFieldType())
            {
                itemSourceField.Add("Sân " + fieldType + " người");
            }
        }
        public void ShowWdSetTimeFrame()
        {
            SetTimeFrameWindow wdSetTime = new SetTimeFrameWindow();
            wdSetTime.ShowDialog();
        }
        public void ShowAddField(HomeWindow home)
        {
            AddFootballFieldWindow wdAddFootballFieldWindow = new AddFootballFieldWindow();
            List<FootballField> footballFields = FootballFieldDAL.Instance.ConvertDBToList();
            try
            {
                wdAddFootballFieldWindow.txtIDField.Text = (footballFields[footballFields.Count() - 1].IdField + 1).ToString();
            }
            catch
            {
                wdAddFootballFieldWindow.txtIDField.Text = "1";
            }
            wdAddFootballFieldWindow.txtName.Text = null;
            wdAddFootballFieldWindow.cboFieldType.Text = null;
            wdAddFootballFieldWindow.ShowDialog();
        }
        public void LoadField(HomeWindow home)
        {
            this.home = home;
            home.grdListField.Visibility = Visibility.Hidden;
            home.grdCardField.Visibility = Visibility.Visible;
            home.cboViews.Text = "Card";
            setItemSourceField();
            LoadCardFieldToView(home);
        }
        public void SelectionChanged(HomeWindow parameter)
        {
            if (parameter.cboViews.SelectedIndex == 0)
            {
                parameter.grdListField.Visibility = Visibility.Visible;
                parameter.grdCardField.Visibility = Visibility.Hidden;
                LoadListFieldToView(parameter.wpListField);
            }
            if (parameter.cboViews.SelectedIndex == 1)
            {
                parameter.grdListField.Visibility = Visibility.Hidden;
                parameter.grdCardField.Visibility = Visibility.Visible;
                LoadCardFieldToView(parameter);
            }
        }
        public void LoadListFieldToView(WrapPanel wrap)
        {
            int i = 1;
            wrap.Children.Clear();
            bool flag = false;
            foreach (var footballField in FootballFieldDAL.Instance.ConvertDBToList())
            {
                FieldDetailsControl temp = new FieldDetailsControl();
                flag = !flag;
                if (flag)
                {
                    temp.grdMain.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
                }
                temp.txbOrderNum.Text = i.ToString();
                i++;
                temp.txbIdField.Text = footballField.IdField.ToString();
                temp.txbFieldName.Text = footballField.Name.ToString();
                temp.txbFieldType.Text = "Sân " + footballField.Type.ToString() + " người";
                if (footballField.Status == 0)
                {
                    temp.txbStatus.Text = "Hỏng";
                }
                else
                {
                    temp.txbStatus.Text = "Tốt";
                }
                if (CurrentAccount.Type == 2)
                {
                    temp.btnDeleteField.IsEnabled = false;
                    temp.btnEditField.IsEnabled = false;
                }
                wrap.Children.Add(temp);
            }
        }
        public void LoadCardFieldToView(HomeWindow parameter)
        {
            parameter.wpCardField.Children.Clear();
            foreach (var field in FootballFieldDAL.Instance.ConvertDBToList())
            {
                FieldControl child = new FieldControl();
                child.txbFieldName.Text = field.Name;
                child.txbIdField.Text = field.IdField.ToString();
                if (field.Status == 1)
                {
                    child.icnError.Visibility = Visibility.Hidden;
                }
                child.txbFieldType.Text = "Sân " + field.Type.ToString() + " người";
                if (CurrentAccount.Type == 2)
                {
                    child.btnDelete.IsEnabled = false;
                    child.btnEdit.IsEnabled = false;
                }
                parameter.wpCardField.Children.Add(child);
            }
        }
        public void Hover(FieldControl parameter)
        {
            parameter.grdMask.Visibility = Visibility.Visible;
            parameter.btnDelete.Visibility = Visibility.Visible;
            parameter.btnEdit.Visibility = Visibility.Visible;
        }
        public void Leave(FieldControl parameter)
        {
            parameter.grdMask.Visibility = Visibility.Hidden;
            parameter.btnEdit.Visibility = Visibility.Hidden;
            parameter.btnDelete.Visibility = Visibility.Hidden;
        }
        public void SaveField(AddFootballFieldWindow parameter)
        {
            //Check các ô có bị null hay không?
            if (string.IsNullOrEmpty(parameter.txtName.Text))
            {
                parameter.txtName.Text = "";
                parameter.txtName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(parameter.cboFieldType.Text))
            {
                parameter.cboFieldType.Focus();
                parameter.cboFieldType.Text = "";
                return;
            }
            if (string.IsNullOrEmpty(parameter.cboStatus.Text))
            {
                parameter.cboStatus.Focus();
                return;
            }

            int status = 1;

            if (parameter.cboStatus.SelectedIndex == 1)
            {
                status = 0;
            }
            List<string> fieldTypes = FootballFieldDAL.Instance.GetFieldType(); // để thêm khung giờ 

            FootballField newField = new FootballField(int.Parse(parameter.txtIDField.Text.ToString()),
                parameter.txtName.Text.ToString(), int.Parse(parameter.cboFieldType.Text.Split(' ')[1]), status, "NULL", 0);
            List<FootballField> fields = FootballFieldDAL.Instance.ConvertDBToList();
            bool isSuccess1 = false;
            bool isSuccess2 = false;
            if (fields.Count == 0 || fields[fields.Count - 1].IdField < newField.IdField)
            {
                //Kiểm tra tên sân có tồn tại hay chưa
                if (FootballFieldDAL.Instance.isExistFieldName(parameter.txtName.Text))
                {
                    CustomMessageBox.Show("Tên sân đã tồn tại! Vui lòng nhập lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    parameter.txtName.Text = "";
                    parameter.txtName.Focus();
                    return;
                }
                //Lưu vào DB
                if (FootballFieldDAL.Instance.AddIntoDB(newField))
                {
                    isSuccess1 = true;
                    CustomMessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);

                    //Hiển thị 
                    if (home.cboViews.SelectedIndex == 0)
                    {
                        FieldDetailsControl control = new FieldDetailsControl();
                        control.txbFieldName.Text = newField.Name;
                        control.txbIdField.Text = newField.IdField.ToString();
                        control.txbFieldType.Text = "Sân " + newField.Type.ToString() + " người";
                        control.txbStatus.Text = newField.Status == 0 ? "Hỏng" : "Tốt";
                        control.txbOrderNum.Text = (home.wpListField.Children.Count + 1).ToString();
                        home.wpListField.Children.Add(control);
                    }
                    else if (home.cboViews.SelectedIndex == 1)
                    {
                        FieldControl control = new FieldControl();
                        control.txbFieldName.Text = newField.Name;
                        control.txbIdField.Text = newField.IdField.ToString();
                        control.txbFieldType.Text = "Sân " + newField.Type.ToString() + " người";
                        if (parameter.cboStatus.SelectedIndex == 0)
                        {
                            control.icnError.Visibility = Visibility.Hidden;
                        }
                        home.wpCardField.Children.Add(control);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Thêm thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                //Kiểm tra tên sân có tồn tại hay không
                if (FootballFieldDAL.Instance.GetFootballFieldById(parameter.txtIDField.Text).Name != parameter.txtName.Text)
                {
                    if (FootballFieldDAL.Instance.isExistFieldName(parameter.txtName.Text))
                    {
                        CustomMessageBox.Show("Tên sân đã tồn tại! Vui lòng nhập lại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                        parameter.txtName.Text = "";
                        parameter.txtName.Focus();
                        return;
                    }
                }
                //Thực hiện update xuống database
                if (FootballFieldDAL.Instance.UpdateField(newField))
                {
                    isSuccess2 = true;
                    CustomMessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    //Cập nhật lên display
                    if (home.cboViews.SelectedIndex == 0)
                    {
                        detailsField.txbFieldName.Text = parameter.txtName.Text;
                        detailsField.txbFieldType.Text = parameter.cboFieldType.Text;
                        detailsField.txbStatus.Text = parameter.cboStatus.Text;
                    }
                    else if (home.cboViews.SelectedIndex == 1)
                    {
                        cardField.txbFieldName.Text = parameter.txtName.Text;
                        cardField.txbFieldType.Text = parameter.cboFieldType.Text;
                        if (parameter.cboStatus.SelectedIndex == 0)
                        {
                            cardField.icnError.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            cardField.icnError.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    CustomMessageBox.Show("Cập nhật thất bại!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            //Thêm khung giờ cho loại sân mới
            if (fieldTypes.Find(x => x == parameter.cboFieldType.Text.Split(' ')[1]) == null && (isSuccess1 || isSuccess2))
            {
                int i = TimeFrameDAL.Instance.GetIdMax();
                foreach (TimeFrame item in TimeFrameDAL.Instance.GetTimeFrame())
                {
                    item.Id = ++i;
                    item.FieldType = newField.Type;
                    item.Price = -1;
                    TimeFrameDAL.Instance.AddTimeFrame(item);
                }
                SetTimeFrameWindow newWinDow = new SetTimeFrameWindow();
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1)
                };
                timer.Tick += (s, e) =>
                {
                    fieldTypes = FootballFieldDAL.Instance.GetFieldType();
                    newWinDow.cboFieldType.SelectedIndex = fieldTypes.IndexOf(newField.Type.ToString());
                    timer.Stop();
                };
                timer.Start();
                newWinDow.ShowDialog();
            }
            parameter.Close();
        }
        public void ClickEditListField(FieldDetailsControl control)
        {
            this.detailsField = control;
            ShowEditField(control.txbIdField.Text);
        }
        public void ClickEditCardField(FieldControl control)
        {
            this.cardField = control;
            ShowEditField(control.txbIdField.Text);
        }
        public void ShowEditField(string idField)
        {
            setItemSourceField();
            AddFootballFieldWindow updateWindow = new AddFootballFieldWindow();
            updateWindow.Title = "Cập nhật sân bóng";
            FootballField footballField = FootballFieldDAL.Instance.GetFootballFieldById(idField);
            updateWindow.txtIDField.Text = idField;
            updateWindow.txtName.Text = footballField.Name;
            updateWindow.txtName.SelectionStart = updateWindow.txtName.Text.Length;
            updateWindow.txtName.SelectionLength = 0;

            updateWindow.cboFieldType.Text = "Sân " + footballField.Type.ToString() + " người";
            int statusIndex = 1;
            if (footballField.Status == 1)
            {
                statusIndex = 0;
            }
            updateWindow.cboStatus.SelectedIndex = statusIndex;
            updateWindow.ShowDialog();
            return;
        }
        public void DeleteListField(FieldDetailsControl control)
        {
            MessageBoxResult result = CustomMessageBox.Show("Xác nhận xóa sân bóng?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                List<FieldInfo> fieldInfos = FieldInfoDAL.Instance.GetFieldInfoByIdField(control.txbIdField.Text);
                if (fieldInfos.Count == 0)
                {
                    //Lưu xuống DB
                    if (FootballFieldDAL.Instance.DeleteField(control.txbIdField.Text)) // cập  nhật isDeleted=1
                    {
                        //Cập nhật lên wrap
                        home.wpListField.Children.Remove(control);
                        if (FootballFieldDAL.Instance.GetFieldType().Find(x => x == control.txbFieldType.Text.Split(' ')[1]) == null)
                        {
                            TimeFrameDAL.Instance.DeleteFieldType(control.txbFieldType.Text.Split(' ')[1]);
                        }
                        bool flag = false;
                        for (int i = 0; i < home.wpListField.Children.Count; i++)
                        {
                            FieldDetailsControl temp = (FieldDetailsControl)home.wpListField.Children[i];
                            flag = !flag;
                            if (flag)
                            {
                                temp.grdMain.Background = (Brush)new BrushConverter().ConvertFromString("#FFFFFF");
                            }
                            else
                            {
                                temp.grdMain.Background = (Brush)new BrushConverter().ConvertFromString("#F4EEFF");
                            }
                            temp.txbOrderNum.Text = (i + 1).ToString();
                        }
                    }
                    else
                    {
                        CustomMessageBox.Show("Sân đang được sử dụng, không được phép xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                return;
            }
        }
        public void DeleteCardField(FieldControl control)
        {
            MessageBoxResult result = CustomMessageBox.Show("Xác nhận xóa sân bóng?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                //Lưu xuống DB
                List<FieldInfo> fieldInfos = FieldInfoDAL.Instance.GetFieldInfoByIdField(control.txbIdField.Text);
                if (fieldInfos.Count == 0)
                {
                    if (FootballFieldDAL.Instance.DeleteField(control.txbIdField.Text)) // cập nhật isDeleted=1
                    {
                        //Cập nhật lên wrap
                        home.wpCardField.Children.Remove(control);
                        if (FootballFieldDAL.Instance.GetFieldType().Find(x => x == control.txbFieldType.Text.Split(' ')[1]) == null)
                        {
                            TimeFrameDAL.Instance.DeleteFieldType(control.txbFieldType.Text.Split(' ')[1]);
                        }
                    }
                }
                else
                {
                    CustomMessageBox.Show("Sân đang được sử dụng, không được phép xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
