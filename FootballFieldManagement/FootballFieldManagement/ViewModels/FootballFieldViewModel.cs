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

        //Field Control
        public ICommand HoverCommand { get; set; }
        public ICommand LeaveCommand { get; set; }
        public ICommand LoadedCommand { get; set; }
        public ICommand EditCardFieldCommand { get; set; }
        public ICommand DeleteCardFieldCommand { get; set; }

        public ICommand SelectionChangedCommand { get; set; }
        public ICommand AddFieldCommand { get; set; }
        public ICommand SetTimeFrameCommand { get; set; }

        private HomeWindow home;
        public HomeWindow Home { get => home; set => home = value; }

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
            AddFieldCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => AddField(parameter));
            SetTimeFrameCommand = new RelayCommand<HomeWindow>((parameter) => true, (parameter) => ShowWdSetTimeFrame());
        }
        public void ShowWdSetTimeFrame()
        {
            SetTimeFrameWindow wdSetTime = new SetTimeFrameWindow();
            wdSetTime.ShowDialog();
        }
        public void AddField(HomeWindow home)
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
                temp.txbUnitPrice.Text = string.Format("{0:N0}", footballField.Price);
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
                string temp = "Sân 5 người";
                if (field.Type == 7)
                {
                    temp = "Sân 7 người";
                }
                if (field.Status == 1)
                {
                    child.icnError.Visibility = Visibility.Hidden;
                }
                child.txbFieldType.Text = temp;
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
            if (string.IsNullOrEmpty(parameter.txtPrice.Text))
            {
                MessageBox.Show("Vui lòng nhập giá sân!");
                parameter.txtPrice.Focus();
                return;
            }

            int type = 5;
            int status = 1;
            if (parameter.cboFieldType.SelectedIndex == 1)
            {
                type = 7;
            }
            if (parameter.cboStatus.SelectedIndex == 1)
            {
                status = 0;
            }
            FootballField newField = new FootballField(int.Parse(parameter.txtIDField.Text.ToString()),
                parameter.txtName.Text.ToString(), type, status,
                ConvertToNumber(parameter.txtPrice.Text.ToString()), "NULL");
            List<FootballField> fields = FootballFieldDAL.Instance.ConvertDBToList();
            if (fields.Count == 0 || fields[fields.Count - 1].IdField < newField.IdField)
            {
                //Thực hiện lưu xuống database
                foreach (var field in FootballFieldDAL.Instance.ConvertDBToList())
                {
                    if (parameter.txtName.Text == field.Name)
                    {
                        MessageBox.Show("Tên sân đã tồn tại!");
                        return;
                    }
                }
                if (FootballFieldDAL.Instance.AddIntoDB(newField))
                {
                    MessageBox.Show("Thêm thành công!");
                    //Cập nhật lên display
                    if (home.cboViews.SelectedIndex == 0)
                    {
                        FieldDetailsControl control = new FieldDetailsControl();
                        control.txbFieldName.Text = parameter.txtName.Text;
                        control.txbFieldType.Text = parameter.cboFieldType.Text;
                        control.txbStatus.Text = parameter.cboStatus.Text;
                        control.txbUnitPrice.Text = parameter.txtPrice.Text;
                        control.txbOrderNum.Text = (home.wpListField.Children.Count + 1).ToString();
                        home.wpListField.Children.Add(control);
                    }
                    else if (home.cboViews.SelectedIndex == 1)
                    {
                        FieldControl control = new FieldControl();
                        control.txbFieldName.Text = parameter.txtName.Text;
                        control.txbFieldType.Text = parameter.cboFieldType.Text;
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
                    MessageBox.Show("Cập nhật thành công!");
                    //Cập nhật lên display
                    if (home.cboViews.SelectedIndex == 0)
                    {
                        detailsField.txbFieldName.Text = parameter.txtName.Text;
                        detailsField.txbFieldType.Text = parameter.cboFieldType.Text;
                        detailsField.txbStatus.Text = parameter.cboStatus.Text;
                        detailsField.txbUnitPrice.Text = parameter.txtPrice.Text;
                    }
                    else if (home.cboViews.SelectedIndex == 1)
                    {
                        cardField.txbFieldName.Text = parameter.txtName.Text;
                        cardField.txbFieldType.Text = parameter.cboFieldType.Text;
                        if (parameter.cboStatus.SelectedIndex == 0)
                        {
                            cardField.icnError.Visibility = Visibility.Hidden;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!");
                }
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
                    int typeIndex = 0;
                    if (footballField.Type == 7)
                    {
                        typeIndex = 1;
                    }
                    updateWindow.cboFieldType.SelectedIndex = typeIndex;
                    int statusIndex = 1;
                    if (footballField.Status == 1)
                    {
                        statusIndex = 0;
                    }
                    updateWindow.cboStatus.SelectedIndex = statusIndex;
                    updateWindow.txtPrice.Text = string.Format("{0:N0}", footballField.Price);
                    updateWindow.txtPrice.SelectionStart = updateWindow.txtPrice.Text.Length;
                    updateWindow.txtPrice.SelectionLength = 0;
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
                //Lưu xuống DB
                foreach (var footballField in FootballFieldDAL.Instance.ConvertDBToList())
                {
                    if (footballField.IdField.ToString() == control.txbIdField.Text)
                    {
                        bool isSuccess1 = FieldInfoDAL.Instance.UpdateIdField(control.txbIdField.Text);
                        bool isSuccess2 = FootballFieldDAL.Instance.DeleteField(control.txbIdField.Text);
                        if (isSuccess1 && isSuccess2 || isSuccess2)
                        {
                            //Cập nhật lên wrap
                            home.wpListField.Children.Remove(control);
                        }
                        return;
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
                foreach (var footballField in FootballFieldDAL.Instance.ConvertDBToList())
                {
                    if (footballField.IdField.ToString() == control.txbIdField.Text)
                    {
                        bool isSuccess1 = FieldInfoDAL.Instance.UpdateIdField(control.txbIdField.Text);
                        bool isSuccess2 = FootballFieldDAL.Instance.DeleteField(control.txbIdField.Text);
                        if (isSuccess1 && isSuccess2 || isSuccess2)
                        {
                            //Cập nhật lên wrap
                            home.wpCardField.Children.Remove(control);
                        }
                        return;
                    }
                }
            }
        }
    }
}
