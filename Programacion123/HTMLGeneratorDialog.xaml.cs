using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        object? webPreviewLastScrollPosition;
        bool webPreviewReady;

        public HTMLGeneratorDialog()
        {
            InitializeComponent();
        }

        public void Init(Subject? _subject)
        {
            generator = new HTMLGenerator();

            generator.LoadOrCreateSettings();

            generator.Subject = _subject;

            var configSubject = WeakReferenceFieldConfiguration<Subject>.CreateForTextBox(TextSubject)
                                                       .WithStorageId(_subject?.StorageId)
                                                       .WithPick(ButtonSubjectPick)
                                                       .WithFormat(EntityFormatContent.Title)
                                                       .WithPickerTitle("Selecciona una programación de módulo")
                                                       .WithBlocker(Blocker);

            subjectController = new(configSubject);

            subjectController.Changed += SubjectController_Changed;


            ComboCoverElement.Items.Add("Logotipo");
            ComboCoverElement.Items.Add("Código del módulo");
            ComboCoverElement.Items.Add("Nombre del módulo");
            ComboCoverElement.Items.Add("Tipo de ciclo");
            ComboCoverElement.Items.Add("Nombre del ciclo");
            ComboCoverElement.Items.Add("Imagen de portada");

            ComboCoverElement.SelectedIndex = 0;

            for(int i = 0; i < 300; i ++)
            {
                ComboCoverElementPositionTop.Items.Add(String.Format("{0:0.00 cm }", 0.1f * i));
                ComboCoverElementPositionLeft.Items.Add(String.Format("{0:0.00 cm }", 0.1f * i));
            }


            ComboDocumentSize.Items.Add("A4");
            ComboDocumentSize.Items.Add("A5");

            ComboDocumentSize.SelectedIndex = (int)generator.DocumentStyle.Size;

            ComboDocumentOrientation.Items.Add("Portrait");
            ComboDocumentOrientation.Items.Add("Landscape");

            ComboDocumentOrientation.SelectedIndex = (int)generator.DocumentStyle.Orientation;

            UpdateDocumentDimensionsUI();

            TextDocumentWidth.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);
            TextDocumentWidth.IsReadOnly = true;
            TextDocumentHeight.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorLocked"]);
            TextDocumentHeight.IsReadOnly = true;

            for(int i = 0; i < 100; i ++)
            {
                ComboDocumentMarginTop.Items.Add(String.Format("{0:0.00 cm }", 0.1f * i));
                ComboDocumentMarginBottom.Items.Add(String.Format("{0:0.00 cm }", 0.1f * i));
                ComboDocumentMarginLeft.Items.Add(String.Format("{0:0.00 cm }", 0.1f * i));
                ComboDocumentMarginRight.Items.Add(String.Format("{0:0.00 cm }", 0.1f * i));
            }

            ComboDocumentMarginTop.SelectedIndex = (int)(generator.DocumentStyle.Margins.Top / 0.1f);
            ComboDocumentMarginBottom.SelectedIndex = (int)(generator.DocumentStyle.Margins.Bottom / 0.1f);
            ComboDocumentMarginLeft.SelectedIndex = (int)(generator.DocumentStyle.Margins.Left / 0.1f);
            ComboDocumentMarginRight.SelectedIndex = (int)(generator.DocumentStyle.Margins.Right/ 0.1f);

            SetBase64ImageInUI(ImageCoverLogo, generator.DocumentStyle.LogoBase64);
            SetBase64ImageInUI(ImageCoverCover, generator.DocumentStyle.CoverBase64);

            ComboTextElement.Items.Add("Encabezado de nivel 1");
            ComboTextElement.Items.Add("Encabezado de nivel 2");
            ComboTextElement.Items.Add("Encabezado de nivel 3");
            ComboTextElement.Items.Add("Encabezado de nivel 4");
            ComboTextElement.Items.Add("Encabezado de nivel 5");
            ComboTextElement.Items.Add("Encabezado de nivel 6");
            ComboTextElement.Items.Add("Texto normal");
            ComboTextElement.Items.Add("Tablas: General");
            ComboTextElement.Items.Add("Tablas: Encabezado de nivel 1");
            ComboTextElement.Items.Add("Tablas: Encabezado de nivel 2");
            ComboTextElement.Items.Add("Portada: Código del módulo");
            ComboTextElement.Items.Add("Portada: Nombre del módulo");
            ComboTextElement.Items.Add("Portada: Tipo de ciclo");
            ComboTextElement.Items.Add("Portada: Nombre del ciclo");
            ComboTextElement.Items.Add("Índice de nivel 1");
            ComboTextElement.Items.Add("Índice de nivel 2");
            ComboTextElement.Items.Add("Índice de nivel 3");

            ComboTextElement.SelectedIndex = 0;

            ComboTextElementFontFamily.Items.Add("Sans Serif");
            ComboTextElementFontFamily.Items.Add("Serif");

            ComboTextElementAlign.Items.Add("Izquierda");
            ComboTextElementAlign.Items.Add("Centro");
            ComboTextElementAlign.Items.Add("Derecha");
            ComboTextElementAlign.Items.Add("Justificar");

            for (int i = 0; i < 100; i ++) { ComboTextElementFontSize.Items.Add(String.Format("{0}pt", i + 1)); }

            AddColorNamesToCombo(ComboTextElementFontColor);

            for(int i = 0; i < 100; i ++)
            {
                ComboTextElementMarginTop.Items.Add(String.Format("{0}pt", i + 1));
                ComboTextElementMarginBottom.Items.Add(String.Format("{0}pt", i + 1));
                ComboTextElementMarginLeft.Items.Add(String.Format("{0}pt", i + 1));
                ComboTextElementMarginRight.Items.Add(String.Format("{0}pt", i + 1));
            }

            ComboTableElement.Items.Add("Celda normal");
            ComboTableElement.Items.Add("Celda de encabezado de nivel 1");
            ComboTableElement.Items.Add("Celda de encabezado de nivel 2");

            ComboTableElement.SelectedIndex = 0;

            AddColorNamesToCombo(ComboTableElementColor);

            for(int i = 0; i < 100; i ++)
            {
                ComboTableElementPaddingTop.Items.Add(String.Format("{0}pt", i + 1));
                ComboTableElementPaddingBottom.Items.Add(String.Format("{0}pt", i + 1));
                ComboTableElementPaddingLeft.Items.Add(String.Format("{0}pt", i + 1));
                ComboTableElementPaddingRight.Items.Add(String.Format("{0}pt", i + 1));
            }

            SetTextElementEventListenersEnabled(true);
            SetTableElementEventListenersEnabled(true);

            UpdateCoverElementStyleUI();
            UpdateTextElementStyleUI();
            UpdateTableElementStyleUI();


            ButtonCoverLogoOpen.Click += ButtonCoverLogoOpen_Click;
            ButtonCoverCoverOpen.Click += ButtonCoverCoverOpen_Click;
            ComboDocumentSize.SelectionChanged += ComboDocumentSize_SelectionChanged;
            ComboDocumentOrientation.SelectionChanged += ComboDocumentOrientation_SelectionChanged;
            ComboDocumentMarginTop.SelectionChanged += ComboDocumentMarginTop_SelectionChanged;
            ComboDocumentMarginBottom.SelectionChanged += ComboDocumentMarginBottom_SelectionChanged;
            ComboDocumentMarginLeft.SelectionChanged += ComboDocumentMarginLeft_SelectionChanged;
            ComboDocumentMarginRight.SelectionChanged += ComboDocumentMarginRight_SelectionChanged;

            
            Validate();

            webPreviewLastScrollPosition = null;
            webPreviewReady = false;
            WebPreview.LoadCompleted += WebPreview_LoadCompleted;
            UpdatePreviewUI();

        }

        private void SetCoverElementEventListenersEnabled(bool enabled)
        {
            if(enabled)
            {
                ComboCoverElement.SelectionChanged += ComboCoverElement_SelectionChanged;
                ComboCoverElementPositionTop.SelectionChanged += ComboCoverElementPositionTop_SelectionChanged;
                ComboCoverElementPositionLeft.SelectionChanged += ComboCoverElementPositionLeft_SelectionChanged;
            }
            else
            {
                ComboCoverElement.SelectionChanged -= ComboCoverElement_SelectionChanged;
                ComboCoverElementPositionTop.SelectionChanged -= ComboCoverElementPositionTop_SelectionChanged;
                ComboCoverElementPositionLeft.SelectionChanged -= ComboCoverElementPositionLeft_SelectionChanged;
            }
            
        }

        private void SetTextElementEventListenersEnabled(bool enabled)
        {
            if(enabled)
            {
                ComboTextElement.SelectionChanged += ComboTextElement_SelectionChanged;
                ComboTextElementFontFamily.SelectionChanged += ComboTextElementFontFamily_SelectionChanged;
                ComboTextElementAlign.SelectionChanged += ComboTextElementAlign_SelectionChanged;
                ComboTextElementFontSize.SelectionChanged += ComboTextElementFontSize_SelectionChanged;
                ComboTextElementFontColor.SelectionChanged += ComboTextElementFontColor_SelectionChanged;
                CheckboxTextElementBold.Checked += CheckboxTextElementBold_Checked;
                CheckboxTextElementBold.Unchecked += CheckboxTextElementBold_Unchecked;
                CheckboxTextElementItalic.Checked += CheckboxTextElementItalic_Checked;
                CheckboxTextElementItalic.Unchecked += CheckboxTextElementItalic_Unchecked;
                CheckboxTextElementUnderscore.Checked += CheckboxTextElementUnderscore_Checked;
                CheckboxTextElementUnderscore.Unchecked += CheckboxTextElementUnderscore_Unchecked;
                ComboTextElementMarginBottom.SelectionChanged += ComboTextElementMarginBottom_SelectionChanged;
                ComboTextElementMarginTop.SelectionChanged += ComboTextElementMarginTop_SelectionChanged;
                ComboTextElementMarginLeft.SelectionChanged += ComboTextElementMarginLeft_SelectionChanged;
                ComboTextElementMarginRight.SelectionChanged += ComboTextElementMarginRight_SelectionChanged;
            }
            else
            {
                ComboTextElement.SelectionChanged -= ComboTextElement_SelectionChanged;
                ComboTextElementFontFamily.SelectionChanged -= ComboTextElementFontFamily_SelectionChanged;
                ComboTextElementAlign.SelectionChanged -= ComboTextElementAlign_SelectionChanged;
                ComboTextElementFontSize.SelectionChanged -= ComboTextElementFontSize_SelectionChanged;
                ComboTextElementFontColor.SelectionChanged -= ComboTextElementFontColor_SelectionChanged;
                CheckboxTextElementBold.Checked -= CheckboxTextElementBold_Checked;
                CheckboxTextElementBold.Unchecked -= CheckboxTextElementBold_Unchecked;
                CheckboxTextElementItalic.Checked -= CheckboxTextElementItalic_Checked;
                CheckboxTextElementItalic.Unchecked -= CheckboxTextElementItalic_Unchecked;
                CheckboxTextElementUnderscore.Checked -= CheckboxTextElementUnderscore_Checked;
                CheckboxTextElementUnderscore.Unchecked -= CheckboxTextElementUnderscore_Unchecked;
                ComboTextElementMarginBottom.SelectionChanged -= ComboTextElementMarginBottom_SelectionChanged;
                ComboTextElementMarginTop.SelectionChanged -= ComboTextElementMarginTop_SelectionChanged;
                ComboTextElementMarginLeft.SelectionChanged -= ComboTextElementMarginLeft_SelectionChanged;
                ComboTextElementMarginRight.SelectionChanged -= ComboTextElementMarginRight_SelectionChanged;
            }
            
        }


        void AddColorNamesToCombo(ComboBox combo)
        {
            combo.Items.Add("Azul Alicia");
            combo.Items.Add("Amatista");
            combo.Items.Add("Blanco Antigüo");
            combo.Items.Add("Agua");
            combo.Items.Add("Aguamarina");
            combo.Items.Add("Azul Celeste");
            combo.Items.Add("Beige");
            combo.Items.Add("Bizcocho");
            combo.Items.Add("Negro");
            combo.Items.Add("Almendra");
            combo.Items.Add("Azul");
            combo.Items.Add("Azul Violeta");
            combo.Items.Add("Marrón");
            combo.Items.Add("Madera Fuerte");
            combo.Items.Add("Azul Cadete");
            combo.Items.Add("Verde Cartujano");
            combo.Items.Add("Marrón Chocolate");
            combo.Items.Add("Naranja Coral");
            combo.Items.Add("Seda de Maíz");
            combo.Items.Add("Azul Añil");
            combo.Items.Add("Carmesí");
            combo.Items.Add("Cyan");
            combo.Items.Add("Magenta Oscuro");
            combo.Items.Add("Azul Oscuro");
            combo.Items.Add("Cyan Oscuro");
            combo.Items.Add("Vara de Oro Oscura");
            combo.Items.Add("Gris Oscuro");
            combo.Items.Add("Verde Oscuro");
            combo.Items.Add("Amarillo Caqui Oscuro");
            combo.Items.Add("Naranja Oscuro");
            combo.Items.Add("Orquídea Oscuro");
            combo.Items.Add("Rojo Oscuro");
            combo.Items.Add("Salmón Oscuro");
            combo.Items.Add("Verde Mar Oscuro");
            combo.Items.Add("Azul Pizarra Oscuro");
            combo.Items.Add("Gris Pizarra Oscuro");
            combo.Items.Add("Turquesa Oscuro");
            combo.Items.Add("Violeta Oscuro");
            combo.Items.Add("Verde Oliva Oscuro");
            combo.Items.Add("Rosa Profundo");
            combo.Items.Add("Azul Cielo Profundo");
            combo.Items.Add("Gris Tenue");
            combo.Items.Add("Azul Capota");
            combo.Items.Add("Rojo Fuego");
            combo.Items.Add("Blanco Floral");
            combo.Items.Add("Verde Bosque");
            combo.Items.Add("Fucsia");
            combo.Items.Add("Gaisnboro");
            combo.Items.Add("Blanco Fantasma");
            combo.Items.Add("Amarillo Oro");
            combo.Items.Add("Vara de Oro Oscura");
            combo.Items.Add("Gris");
            combo.Items.Add("Verde");
            combo.Items.Add("Verde Amarillento");
            combo.Items.Add("Miel Crema");
            combo.Items.Add("Rosa Cálido");
            combo.Items.Add("Rojo Indio");
            combo.Items.Add("Indigo");
            combo.Items.Add("Blanco Marfíl");
            combo.Items.Add("Amarillo Caqui");
            combo.Items.Add("Espliego");
            combo.Items.Add("Lavanda");
            combo.Items.Add("Verde Césped");
            combo.Items.Add("Amarillo Limón");
            combo.Items.Add("Azul Claro");
            combo.Items.Add("Coral Suave");
            combo.Items.Add("Cyan Suave");
            combo.Items.Add("Amarillo Manzana Suave");
            combo.Items.Add("Gris Claro");
            combo.Items.Add("Verde Claro");
            combo.Items.Add("Rosa Suave");
            combo.Items.Add("Salmón Suave");
            combo.Items.Add("Verde Mar Claro");
            combo.Items.Add("Azul Cielo Claro");
            combo.Items.Add("Azul Acero Claro");
            combo.Items.Add("Amarillo Suave");
            combo.Items.Add("Amarillo Suave");
            combo.Items.Add("Verde Lima");
            combo.Items.Add("Blanco Lino");
            combo.Items.Add("Gris Pizarra Claro");
            combo.Items.Add("Magenta");
            combo.Items.Add("Castaño");
            combo.Items.Add("Aguamarina Medio");
            combo.Items.Add("Azul Medio");
            combo.Items.Add("Orquídea Medio");
            combo.Items.Add("Púrpura Medio");
            combo.Items.Add("Verde Mar Medio");
            combo.Items.Add("Azul Pizarra Medio");
            combo.Items.Add("Verde Primavera Medio");
            combo.Items.Add("Turquesa Medio");
            combo.Items.Add("Medio Violeta Rojo");
            combo.Items.Add("Azul Media Noche");
            combo.Items.Add("Rosa Palo");
            combo.Items.Add("Amarillo Mocasin");
            combo.Items.Add("Marrón Navaja");
            combo.Items.Add("Azul Naval");
            combo.Items.Add("Blanco Cordón Viejo");
            combo.Items.Add("Oliva");
            combo.Items.Add("Verde Oliva");
            combo.Items.Add("Naranja Puro");
            combo.Items.Add("Naranja Rojizo");
            combo.Items.Add("Orquídea");
            combo.Items.Add("Amarillo Oro Pálido");
            combo.Items.Add("Verde Pálido");
            combo.Items.Add("Turquesa Pastel");
            combo.Items.Add("Rosa Pastel");
            combo.Items.Add("Amarillo Papaya");
            combo.Items.Add("Amarillo Melocotón");
            combo.Items.Add("Marrón Perú");
            combo.Items.Add("Rosa");
            combo.Items.Add("Ciruelo");
            combo.Items.Add("Azul Pálido");
            combo.Items.Add("Púrpura");
            combo.Items.Add("Rojo Puro");
            combo.Items.Add("Marrón Atractivo");
            combo.Items.Add("Azul Real");
            combo.Items.Add("Marrón Silla");
            combo.Items.Add("Concha de Mar");
            combo.Items.Add("Salmón Oscuro");
            combo.Items.Add("Marrón Arenoso");
            combo.Items.Add("Verde Mar");
            combo.Items.Add("Marrón Siena");
            combo.Items.Add("Plata");
            combo.Items.Add("Azul Cielo");
            combo.Items.Add("Azul Pizarra");
            combo.Items.Add("Gris Pizarra");
            combo.Items.Add("Blanco Nieve");
            combo.Items.Add("Verde Primavera");
            combo.Items.Add("Azul Acero");
            combo.Items.Add("Marrón bronceado");
            combo.Items.Add("Cárcel");
            combo.Items.Add("Cardo");
            combo.Items.Add("Tomate");
            combo.Items.Add("Turquesa");
            combo.Items.Add("Violeta Oscuro");
            combo.Items.Add("Marrón Trigo");
            combo.Items.Add("Blanco");
            combo.Items.Add("Blanco Humo");
            combo.Items.Add("Amarillo Puro");
            combo.Items.Add("Amarillo Verdoso");
        }

        void UpdateCoverElementStyleUI()
        {
            DocumentCoverElementId id = (DocumentCoverElementId)ComboCoverElement.SelectedIndex;
            if(!generator.CoverElementStyles.ContainsKey(id)) { generator.CoverElementStyles.Add(id, new DocumentCoverElementStyle()); }

            DocumentCoverElementStyle style = generator.CoverElementStyles[id];

            SetCoverElementEventListenersEnabled(false);

            ComboCoverElementPositionTop.SelectedIndex = (int)(style.Position.Top / 0.1f);
            ComboCoverElementPositionLeft.SelectedIndex = (int)(style.Position.Left / 0.1f);

            SetCoverElementEventListenersEnabled(true);
        }


        void UpdateTextElementStyleUI()
        {
            DocumentTextElementId id = (DocumentTextElementId)ComboTextElement.SelectedIndex;
            if(!generator.TextElementStyles.ContainsKey(id)) { generator.TextElementStyles.Add(id, new DocumentTextElementStyle()); }

            DocumentTextElementStyle style = generator.TextElementStyles[id];

            SetTextElementEventListenersEnabled(false);

            CheckboxTextElementBold.IsChecked = style.Bold;
            CheckboxTextElementItalic.IsChecked = style.Italic;
            CheckboxTextElementUnderscore.IsChecked = style.Underscore;

            ComboTextElementFontFamily.SelectedIndex = (int)style.FontFamily;
            ComboTextElementAlign.SelectedIndex = (int)style.Align;
            ComboTextElementFontColor.SelectedIndex = (int)style.FontColor;
            UpdateDocumentTextElementColorUI();

            ComboTextElementFontSize.SelectedIndex = (int)(style.FontSize - 1);

            ComboTextElementMarginTop.SelectedIndex = style.Margins.Top;
            ComboTextElementMarginBottom.SelectedIndex = style.Margins.Bottom;
            ComboTextElementMarginLeft.SelectedIndex = style.Margins.Left;
            ComboTextElementMarginRight.SelectedIndex = style.Margins.Right;

            SetTextElementEventListenersEnabled(true);
        }

        private void UpdateTableElementStyleUI()
        {
            DocumentTableElementId id = (DocumentTableElementId)ComboTableElement.SelectedIndex;
            if(!generator.TableElementStyles.ContainsKey(id)) { generator.TableElementStyles.Add(id, new DocumentTableElementStyle()); }

            DocumentTableElementStyle style = generator.TableElementStyles[id];

            SetTableElementEventListenersEnabled(false);

            ComboTableElementColor.SelectedIndex = (int)style.BackgroundColor;
            UpdateDocumentTableElementColorUI();

            ComboTableElementPaddingTop.SelectedIndex = style.Padding.Top;
            ComboTableElementPaddingBottom.SelectedIndex = style.Padding.Bottom;
            ComboTableElementPaddingLeft.SelectedIndex = style.Padding.Left;
            ComboTableElementPaddingRight.SelectedIndex = style.Padding.Right;

            SetTableElementEventListenersEnabled(true);
        }

        private void UpdateDocumentTableElementColorUI()
        {
            int r; int g; int b;
            generator.GetRGBFromColor((DocumentElementColor)ComboTableElementColor.SelectedIndex, out r, out g, out b);
            RectangleTableElementColor.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)r, (byte)g, (byte)b));
        }

        private void SetTableElementEventListenersEnabled(bool enabled)
        {
            if(enabled)
            {
                ComboTableElement.SelectionChanged += ComboTableElement_SelectionChanged; ;
                ComboTableElementColor.SelectionChanged += ComboTableElementColor_SelectionChanged;
                ComboTableElementPaddingTop.SelectionChanged += ComboTableElementPaddingTop_SelectionChanged;
                ComboTableElementPaddingBottom.SelectionChanged += ComboTableElementPaddingBottom_SelectionChanged;
                ComboTableElementPaddingLeft.SelectionChanged += ComboTableElementPaddingLeft_SelectionChanged;
                ComboTableElementPaddingRight.SelectionChanged += ComboTableElementPaddingRight_SelectionChanged;
            }
            else
            {
                ComboTableElement.SelectionChanged -= ComboTableElement_SelectionChanged; ;
                ComboTableElementColor.SelectionChanged -= ComboTableElementColor_SelectionChanged;
                ComboTableElementPaddingTop.SelectionChanged -= ComboTableElementPaddingTop_SelectionChanged;
                ComboTableElementPaddingBottom.SelectionChanged -= ComboTableElementPaddingBottom_SelectionChanged;
                ComboTableElementPaddingLeft.SelectionChanged -= ComboTableElementPaddingLeft_SelectionChanged;
                ComboTableElementPaddingRight.SelectionChanged -= ComboTableElementPaddingRight_SelectionChanged;
            }
        }

        private void ComboTextElementAlign_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboCoverElementPositionTop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboCoverElementPositionLeft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTableElementPaddingRight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTableElementPaddingLeft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTableElementPaddingBottom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTableElementPaddingTop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTableElementColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdateDocumentTableElementColorUI();
            UpdatePreviewUI();
        }

        private void ComboTableElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTableElementStyleUI();
        }

        private void ComboTextElementMarginTop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTextElementMarginBottom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTextElementMarginLeft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTextElementMarginRight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void CheckboxTextElementUnderscore_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void CheckboxTextElementUnderscore_Checked(object sender, RoutedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void CheckboxTextElementItalic_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void CheckboxTextElementItalic_Checked(object sender, RoutedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void CheckboxTextElementBold_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void CheckboxTextElementBold_Checked(object sender, RoutedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTextElementFontColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdateDocumentTextElementColorUI();
            UpdatePreviewUI();
        }

        private void ComboTextElementFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboTextElementFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboCoverElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCoverElementStyleUI();
        }

        private void ComboTextElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTextElementStyleUI();
        }


        private void ComboDocumentMarginRight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboDocumentMarginLeft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboDocumentMarginBottom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboDocumentMarginTop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdatePreviewUI();
        }

        private void ComboDocumentOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdateDocumentDimensionsUI();
            UpdatePreviewUI();
        }

        private void ComboDocumentSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenerator();
            Validate();
            UpdateDocumentDimensionsUI();
            UpdatePreviewUI();
        }

        void UpdatePreviewUI()
        {
            if(webPreviewReady)
            {
                webPreviewLastScrollPosition = WebPreview.InvokeScript("getVerticalScrollPosition");
            }
            string html = generator.GenerateHTML(true);
            WebPreview.NavigateToString(html);
        }

        void WebPreview_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            // https://stackoverflow.com/questions/5496549/how-to-inject-css-in-webbrowser-control

            HTMLDocument htmlDocument = (HTMLDocument)WebPreview.Document;
            IHTMLStyleSheet style = htmlDocument.createStyleSheet("",0);

            style.cssText = generator.GenerateCSS();

            if(webPreviewReady && webPreviewLastScrollPosition != null)
            {
                WebPreview.InvokeScript("setVerticalScrollPosition", webPreviewLastScrollPosition.ToString());
            }

            webPreviewReady = true;

        }


        string? GetBase64ImageFromUI(Image image)
        {
            if(image.Source != null)
            {
                //https://stackoverflow.com/questions/553611/wpf-image-to-byte

                MemoryStream memoryStream = new();              
                PngBitmapEncoder encoder = new();
                BitmapFrame frame = BitmapFrame.Create((BitmapImage)image.Source);
                encoder.Frames.Add(frame);
                encoder.Save(memoryStream);

                return Convert.ToBase64String(memoryStream.ToArray());
            }
            else
            {
                return null;
            }
        }

        void SetBase64ImageInUI(Image image, string? imageBase64)
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

                image.Source = bitmap;

            }
            else
            {
                image.Source = null;
            }
        }

        void UpdateDocumentDimensionsUI()
        {
            float width;
            float height;
            generator.GetDimensionsFromSizeAndOrientation(
                   (DocumentSize)ComboDocumentSize.SelectedIndex,
                   (DocumentOrientation)ComboDocumentOrientation.SelectedIndex,
                   out width,
                   out height
            );

            TextDocumentWidth.Text = String.Format("{0:0.00} cm", width);
            TextDocumentHeight.Text = String.Format("{0:0.00} cm", height);
        }

        void UpdateDocumentTextElementColorUI()
        {
            int r; int g; int b;
            generator.GetRGBFromColor((DocumentElementColor)ComboTextElementFontColor.SelectedIndex, out r, out g, out b);
            RectangleTextElementColor.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)r, (byte)g, (byte)b));
        }



        private void ButtonCoverLogoOpen_Click(object sender, RoutedEventArgs e)
        {
            ButtonCoverImageOpenClick(ImageCoverLogo);

        }

        private void ButtonCoverCoverOpen_Click(object sender, RoutedEventArgs e)
        {
            ButtonCoverImageOpenClick(ImageCoverCover);
        }

        void ButtonCoverImageOpenClick(Image target)
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
                SetBase64ImageInUI(target, base64);
                
                UpdateGenerator();
                Validate();

                UpdatePreviewUI();

            }
        }

        private void SubjectController_Changed(WeakReferenceFieldController<Subject, EntityPicker<Subject>> controller)
        {
            UpdateGenerator();
            Validate();

            UpdatePreviewUI();
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

            generator.CoverElementStyles[(DocumentCoverElementId)ComboCoverElement.SelectedIndex] = new()
                                      {
                                        Position = new()
                                                  { 
                                                    Top = ComboCoverElementPositionTop.SelectedIndex * 0.1f,
                                                    Left = ComboCoverElementPositionLeft.SelectedIndex * 0.1f,
                                                  }

                                      };


            generator.DocumentStyle = new()
                                      {
                                        LogoBase64 = GetBase64ImageFromUI(ImageCoverLogo),
                                        CoverBase64 = GetBase64ImageFromUI(ImageCoverCover),
                                        Orientation = (DocumentOrientation)ComboDocumentOrientation.SelectedIndex,
                                        Size = (DocumentSize)ComboDocumentSize.SelectedIndex,
                                        Margins = new()
                                                  {
                                                    Top = ComboDocumentMarginTop.SelectedIndex * 0.1f,
                                                    Bottom = ComboDocumentMarginBottom.SelectedIndex * 0.1f,
                                                    Left = ComboDocumentMarginLeft.SelectedIndex * 0.1f,
                                                    Right = ComboDocumentMarginRight.SelectedIndex * 0.1f,
                                                  }                                        
                                      };

            generator.TextElementStyles[(DocumentTextElementId)ComboTextElement.SelectedIndex] = new()
                                      {
                                        Bold = CheckboxTextElementBold.IsChecked.GetValueOrDefault(),
                                        Italic = CheckboxTextElementItalic.IsChecked.GetValueOrDefault(),
                                        Underscore = CheckboxTextElementUnderscore.IsChecked.GetValueOrDefault(),
                                        FontColor = (DocumentElementColor)ComboTextElementFontColor.SelectedIndex,
                                        FontSize = ComboTextElementFontSize.SelectedIndex + 1,
                                        FontFamily = (DocumentTextElementFontFamily)ComboTextElementFontFamily.SelectedIndex,
                                        Align = (DocumentTextElementAlign)ComboTextElementAlign.SelectedIndex,
                                        Margins = new()
                                                  { 
                                                    Top = ComboTextElementMarginTop.SelectedIndex,
                                                    Bottom = ComboTextElementMarginBottom.SelectedIndex,
                                                    Left = ComboTextElementMarginLeft.SelectedIndex,
                                                    Right = ComboTextElementMarginRight.SelectedIndex
                                                  }

                                      };

            generator.TableElementStyles[(DocumentTableElementId)ComboTableElement.SelectedIndex] = new()
                                      {
                                        BackgroundColor = (DocumentElementColor)ComboTableElementColor.SelectedIndex,
                                        Padding = new()
                                                  { 
                                                    Top = ComboTableElementPaddingTop.SelectedIndex,
                                                    Bottom = ComboTableElementPaddingBottom.SelectedIndex,
                                                    Left = ComboTableElementPaddingLeft.SelectedIndex,
                                                    Right = ComboTableElementPaddingRight.SelectedIndex
                                                  }

                                      };

            generator.SaveSettings();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new();
            saveFile.Title = "Guardar programación como";
            saveFile.Filter = "Ficheros html (*.html)|*.html|Todos los ficheros (*.*)|*.*";
            saveFile.FilterIndex = 0;
            saveFile.OverwritePrompt = true;
            saveFile.FileName = "Programación didáctica";

            Blocker.Visibility = Visibility.Visible;

            if(saveFile.ShowDialog().GetValueOrDefault())
            {
                generator.Generate(saveFile.FileName);

                Process process = new();
                process.StartInfo = new ProcessStartInfo(saveFile.FileName);
                process.StartInfo.UseShellExecute = true;
                process.Start();

                Close();
            }
            
            Blocker.Visibility = Visibility.Hidden;


            
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
