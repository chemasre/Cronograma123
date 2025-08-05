using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using MSHTML;


namespace Programacion123
{
    /// <summary>
    /// Lógica de interacción para HTMLGeneratorSettings.xaml
    /// </summary>
    public partial class HTMLGeneratorDialog : Window
    {
        WeakReferenceFieldController<Subject, EntityPicker<Subject> > subjectController;

        HTMLGenerator generator;

        public HTMLGeneratorDialog()
        {
            InitializeComponent();
        }

        public void Init(Subject? _subject)
        {
            var configSubject = WeakReferenceFieldConfiguration<Subject>.CreateForTextBox(TextSubject)
                                                       .WithStorageId(_subject?.StorageId)
                                                       .WithPick(ButtonSubjectPick)
                                                       .WithFormat(EntityFormatContent.title)
                                                       .WithPickerTitle("Selecciona una programación de módulo")
                                                       .WithBlocker(Blocker);

            subjectController = new(configSubject);

            subjectController.Changed += SubjectController_Changed;

            generator = new HTMLGenerator();
            generator.LoadOrCreate("HTMLGenerator.json");

            TextAdditionalCss.Text = generator.AdditionalCss;

            SetBase64LogoImageInUI(generator.LogoBase64);

            generator.Subject = _subject;

            ButtonLogoOpen.Click += ButtonLogoOpen_Click;
            TextAdditionalCss.TextChanged += TextAdditionalCss_TextChanged;

            Validate();

            WebPreview.LoadCompleted += WebPreview_LoadCompleted;
            UpdateWebPreviewUI();

        }

        private void TextAdditionalCss_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdateWebPreviewUI();
        }

        void UpdateWebPreviewUI()
        {
            string html = generator.GenerateHTML();
            WebPreview.NavigateToString(html);
        }

        void WebPreview_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // https://stackoverflow.com/questions/5496549/how-to-inject-css-in-webbrowser-control

            HTMLDocument htmlDocument = (HTMLDocument)WebPreview.Document;
            IHTMLStyleSheet style = htmlDocument.createStyleSheet("",0);
            style.cssText = generator.AdditionalCss + " body {font-size:8pt; }";
        }

        string? GetBase64LogoImageFromUI()
        {
            if(ImageLogo.Source != null)
            {
                //https://stackoverflow.com/questions/553611/wpf-image-to-byte

                MemoryStream memoryStream = new();              
                PngBitmapEncoder encoder = new();
                BitmapFrame frame = BitmapFrame.Create((BitmapImage)ImageLogo.Source);
                encoder.Frames.Add(frame);
                encoder.Save(memoryStream);

                return Convert.ToBase64String(memoryStream.ToArray());
            }
            else
            {
                return null;
            }
        }

        void SetBase64LogoImageInUI(string? imageBase64)
        {
            if(imageBase64 != null)
            {
                // https://stackoverflow.com/questions/593388/how-do-i-read-a-base64-image-in-wpf

                byte[] bytes = Convert.FromBase64String(imageBase64);
                MemoryStream stream = new MemoryStream(bytes);
                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImageLogo.Source = bitmap;

            }
            else
            {
                ImageLogo.Source = null;
            }

        }

        private void ButtonLogoOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new();
            openFile.Title = "Elige imagen para usar como logotipo";
            openFile.Filter = "Ficheros png (*.png)|*.png|Ficheros jpg (*.jpg)|*.jpg|Ficheros bmp (*.bmp)|*.bmp|Todos los ficheros (*.*)|*.*";
            openFile.FilterIndex = 0;

            Blocker.Visibility = Visibility.Visible;

            bool opened = false;

            if(openFile.ShowDialog().GetValueOrDefault()) { opened = true; }

            Blocker.Visibility = Visibility.Hidden;

            if(opened)
            {
                byte[] bytes = File.ReadAllBytes(openFile.FileName);
                string base64 = Convert.ToBase64String(bytes);
                SetBase64LogoImageInUI(base64);
                
                UpdateGenerator();
                Validate();

                UpdateWebPreviewUI();

            }

        }

        private void SubjectController_Changed(WeakReferenceFieldController<Subject, EntityPicker<Subject>> controller)
        {
            UpdateGenerator();
            Validate();

            UpdateWebPreviewUI();
        }

        void Validate()
        {
            GeneratorValidationResult result = generator.Validate();

            string colorResource = (result.code == GeneratorValidationCode.success ? "ColorValid" : "ColorInvalid");
            BorderValidation.Background = new SolidColorBrush((Color)Application.Current.Resources[colorResource]);
            TextValidation.Text = result.ToString();

        }

        void UpdateGenerator()
        {
            generator.Subject = subjectController.GetEntity();
            generator.AdditionalCss = TextAdditionalCss.Text;
            generator.LogoBase64 = GetBase64LogoImageFromUI();

            generator.Save("HTMLGenerator.json");
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            generator.Generate("Test.html");

            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



    }
}
