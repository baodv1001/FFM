using FootballFieldManagement.Views;
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
            setItemSourceField();
            wdAddFootballFieldWindow.ShowDialog();
        }
        public void LoadField(HomeWindow home)
        {
            this.home = home;
            home.grdListField.Visibility = Visibility.Hidden;
            home.grdCardField.Visibility = Visibility.Visible;
            home.cboViews.Text = "Card";
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
                MessageBox.Show("Vui lòng nhập tên sân!");
                parameter.txtName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(parameter.cboFieldType.Text))
            {
                MessageBox.Show("Vui lòng chọn loại sân!");
                parameter.cboFieldType.Focus();
                return;
            }
            if (string.IsNullOrEmpty(parameter.cboStatus.Text))
            {
                MessageBox.Show("Vui lòng chọn trạng thái sân!");
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
                //Thực hiện lưu xuống database
                if (FootballFieldDAL.Instance.CheckFieldName(parameter.txtName.Text))
                {
                    MessageBox.Show("Tên sân đã tồn tại!");
                    return;
                }
                if (FootballFieldDAL.Instance.AddIntoDB(newField))
                {
                    isSuccess1 = true;
                    MessageBox.Show("Thêm thành công!", "Thông báo");

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
                    MessageBox.Show("Thêm thất bại!");
                }
            }
            else
            {
                //Thực hiện update xuống database
                if (FootballFieldDAL.Instance.UpdateField(newField))
                {
                    isSuccess2 = true;
                    MessageBox.Show("Cập nhật thành công!");
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
                    MessageBox.Show("Cập nhật thất bại!");
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
            AddFootballFieldWindow updateWindow = new AddFootballFieldWindow();
            foreach (var footballField in FootballFieldDAL.Instance.ConvertDBToList())
            {
                if (footballField.IdField.ToString() == idField)
                {
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
                    updateWindow.Title = "Cập nhật thông tin sân";
                    updateWindow.ShowDialog();
                    return;
                }
            }
        }
        public void DeleteListField(FieldDetailsControl control)
        {
            MessageBoxResult result = MessageBox.Show("Xác nhận xóa sân bóng?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                    }
                    else
                    {
                        MessageBox.Show("Sân đang được sử dụng, không được phép xóa!");
                    }
                }
            }
        }
        public void DeleteCardField(FieldControl control)
        {
            MessageBoxResult result = MessageBox.Show("Xác nhận xóa sân bóng?", "Thông báo", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                    MessageBox.Show("Sân đang được sử dụng, không được phép xóa!");
                }
            }
        }
    }
}
