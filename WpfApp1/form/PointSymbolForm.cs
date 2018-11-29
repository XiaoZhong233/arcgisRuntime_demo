using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1.form
{

    public partial class PointSymbolForm: Form
    {
        #region 私有变量
        Layer layer;
        private List<SimpleMarkerSymbolStyle> style = new List<SimpleMarkerSymbolStyle>();
        //用户选择的样式
        private SimpleMarkerSymbolStyle selectStyle = SimpleMarkerSymbolStyle.Circle;
        //用户选择的大小
        private int size = 1;
        //用户选择的颜色
        private Color color = Color.Black;
        //图例
        SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
        PictureMarkerSymbol pictureMarkerSymbol;
        #endregion
        
        #region 属性
        public SimpleMarkerSymbol SimpleMarkerSymbol
        {
            get
            {
                return this.simpleMarkerSymbol;
            }
        }
        
        #endregion
        
        #region 初始化
        public PointSymbolForm(Layer layer)
        {
            InitializeComponent();
            initData();
            this.layer = layer;
            initLegend();
            init();
        }

        public PointSymbolForm()
        {
            InitializeComponent();
            initData();
            init();
            initControls();
        }


        /// <summary>
        /// 如果未传入图层
        /// </summary>
        private  void initControls()
        {
            pictureMarkerSymbol = null;
            simple_RB.Checked = true;
            panel2.Visible = false;
            panel2.Enabled = false;
            simpleMarkerSymbol.Color = Color.Red;
            colorBtn.BackColor = simpleMarkerSymbol.Color;
            sizeUpDown.Value = 20;
        }
        
        /// <summary>
        /// 由传进来的layer初始化simpleMaker的样式
        /// </summary>
        private async void initLegend()
        {
            //初始化点样式
            FeatureLayer feature = layer as FeatureLayer;
            SimpleRenderer simpleRenderer = feature.Renderer as SimpleRenderer;
            if (simpleRenderer != null)
            {
                //需要注意的是 这里不应该直接引用传递 否则在未确认的情况下也会影响到点的样式
                simpleMarkerSymbol = simpleRenderer.Symbol.Clone() as SimpleMarkerSymbol;
                //如果为空则说明Symbol不为简单点样式
                if (simpleMarkerSymbol == null)
                {
                    //因为后面需要订阅事件 所以不能为null
                    simpleMarkerSymbol = new SimpleMarkerSymbol();
                    pictureMarkerSymbol = simpleRenderer.Symbol.Clone() as PictureMarkerSymbol;
                    if (pictureMarkerSymbol != null)
                    {
                        Esri.ArcGISRuntime.UI.RuntimeImage image=await pictureMarkerSymbol.CreateSwatchAsync(Color.WhiteSmoke);
                        System.Windows.Media.ImageSource imageSource = await Esri.ArcGISRuntime.UI.RuntimeImageExtensions.ToImageSourceAsync(image);
                        Image result = imageWpfToGDI(imageSource);
                        previewBox.Image = result;
                        previewBox.SizeMode = PictureBoxSizeMode.Zoom;
                        simple_RB.Checked = false;
                        customPic_RB.Checked = true;
                        StyleCombox.Enabled = false;
                        colorBtn.Enabled = false;
                        sizeUpDown.Value = (decimal)pictureMarkerSymbol.Width+1;
                        transparencyControl.Value = MainWindow.getAlpha(pictureMarkerSymbol.Opacity);
                        
                    }
                    
                }
                else
                {
                   
                    int index = findSelectIndex(x =>
                    {
                        return simpleMarkerSymbol.Style.ToString().Equals(x.ToString());
                    });
                    int count = ((List<SimpleMarkerSymbolStyle>)StyleCombox.DataSource).Count;
                    //设置传入的layer的样式
                    simple_RB.Checked = true;
                    StyleCombox.SelectedIndex = index;
                    colorBtn.BackColor = simpleMarkerSymbol.Color;
                    sizeUpDown.Value = (decimal)simpleMarkerSymbol.Size;
                    transparencyControl.Value = simpleMarkerSymbol.Color.A;
                    await refreshPreviewAsync();
                }

            }
            //如果Layer是别的数据类型
            else
            {

            }
        }

        /// <summary>
        /// 初始化数据源
        /// </summary>
        private void initData()
        {
            style.Add(SimpleMarkerSymbolStyle.Circle);
            style.Add(SimpleMarkerSymbolStyle.Cross);
            style.Add(SimpleMarkerSymbolStyle.Diamond);
            style.Add(SimpleMarkerSymbolStyle.Square);
            style.Add(SimpleMarkerSymbolStyle.Triangle);
            style.Add(SimpleMarkerSymbolStyle.X);
            StyleCombox.DataSource = style;
        }

        /// <summary>
        /// 初始化控件，进行事件订阅
        /// </summary>
        private void init()
        {
            
            StyleCombox.SelectedIndexChanged += (s, e) =>
            {
                ComboBox c = s as ComboBox;
                if (c != null)
                {
                    selectStyle = style[c.SelectedIndex];
                    simpleMarkerSymbol.Style = selectStyle;
                    
                }
            };

            sizeUpDown.KeyPress +=  async (s, e) =>
            {
                await refreshPreviewAsync();
            };



            sizeUpDown.ValueChanged +=  async (s, e) =>
            {
                NumericUpDown numericUpDown = s as NumericUpDown;
                if (numericUpDown != null)
                {
                    size = (int)numericUpDown.Value;
                    simpleMarkerSymbol.Size = size;
                    //previewBox.Scale(new SizeF(size, size));
                    if (pictureMarkerSymbol != null)
                    {
                        pictureMarkerSymbol.Width = size;
                        pictureMarkerSymbol.Height = size;
                        await refreshPreviewAsync();
                    }
                }
            };

            simple_RB.CheckedChanged += async (s, e) =>
            {
                if (simple_RB.Checked)
                {
                    previewBox.Image = null;
                    StyleCombox.Enabled = true;
                    colorBtn.Enabled = true;
                    cusPictureBtn.Enabled = false;
                    cusPictureBtn.Visible = false;
                    await refreshPreviewAsync();
                }
                else
                {
                    previewBox.Image = null;
                    StyleCombox.Enabled = false;
                    colorBtn.Enabled = false;
                    cusPictureBtn.Enabled = true;
                    cusPictureBtn.Visible = true;
                }
            };

            cusPictureBtn.Click += (s, e) =>
            {
                openPicDialog((ofd) =>
                {
                    previewBox.Load(ofd.FileName);
                    previewBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureMarkerSymbol = new PictureMarkerSymbol(new Uri(ofd.FileName));
                    cusPictureBtn.Visible = false;
                    cusPictureBtn.Enabled = false;
                    sizeUpDown.Value = (decimal)pictureMarkerSymbol.Height+1;
                });
            };

            simpleMarkerSymbol.PropertyChanged += async (s, e) =>
            {
                if(simple_RB.Checked)
                await refreshPreviewAsync();
            };

            //透明度改变
            transparencyControl.ValueChanged += async (s, e) =>
            {
                if (simple_RB.Checked)
                {
                    Color c = simpleMarkerSymbol.Color;
                    simpleMarkerSymbol.Color = Color.FromArgb(e, c.R, c.G, c.B);
                }
                else
                {
                    pictureMarkerSymbol.Opacity = transparencyControl.getOpacity();
                    //这里有可能因为线程问题抛出异常，需要捕捉异常,可能是用户上传的图片过大或者不合格，关掉窗口 让用户重新上传
                    try
                    {
                        await refreshPreviewAsync();
                    }
                    catch(Exception error)
                    {
                        MessageBox.Show("错误：\n " +error.Message+ "\n图片不合格，请重新上传");
                        Close();
                    }
                    
                }
            };
        }


        #region 事件处理
        /// <summary>
        /// 打开图片对话框，然后做一些事
        /// </summary>
        /// <param name="action"></param>
        public void openPicDialog(Action<OpenFileDialog> action)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择图像文件";
            ofd.Filter = "图片文件|*.bmp;*.jpg;*.jpeg;*.gif;*.png";
            ofd.RestoreDirectory = true;
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                action(ofd);
            }
        }


        /// <summary>
        /// 窗体加载绑定数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PointSymbolForm_Load(object sender, EventArgs e)
        {
            //StyleCombox.DataSource = style;

        }
        #endregion



     
        /// <summary>
        /// okBtn事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okBtn_Click(object sender, EventArgs e)
        {
            SimpleRenderer simpleRenderer = new SimpleRenderer(simpleMarkerSymbol);
            FeatureLayer feature = layer as FeatureLayer;
            if (feature != null && simple_RB.Checked)
            {
                feature.Renderer = simpleRenderer;
                Close();
            }
            else if(feature != null && customPic_RB.Checked)
            {
                if (pictureMarkerSymbol == null)
                {
                    MessageBox.Show("请选择图片");
                    return;
                }
                simpleRenderer.Symbol = pictureMarkerSymbol;
                feature.Renderer = simpleRenderer;
                Close();
            }
            //其他情况
            else
            {
                Close();
            }
        }

        /// <summary>
        /// colorBtn事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorBtn_Click(object sender, EventArgs e)
        {
            Color color = colorBtn.BackColor;
            DialogResult result = colorDialog.ShowDialog();
            if (result == DialogResult.OK && simpleMarkerSymbol!=null)
            {
                //需要保持A通道
                transparencyControl.Value = colorDialog.Color.A;
                colorBtn.BackColor = colorDialog.Color;
                color = colorDialog.Color;
                simpleMarkerSymbol.Color = color;
            }
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 刷新预览图
        /// 适用于simpleMaker状态
        /// </summary>
        private async Task refreshPreviewAsync()
        {
            //if (simpleMarkerSymbol == null)
            //    return;
            if (simple_RB.Checked )
            {
                if (simpleMarkerSymbol == null)
                    return;
                Esri.ArcGISRuntime.UI.RuntimeImage image = await simpleMarkerSymbol.CreateSwatchAsync(Color.WhiteSmoke);
                System.Windows.Media.ImageSource imageSource = await Esri.ArcGISRuntime.UI.RuntimeImageExtensions.ToImageSourceAsync(image);
                Image result = imageWpfToGDI(imageSource);
                previewBox.Image = result;
                previewBox.SizeMode = PictureBoxSizeMode.AutoSize | PictureBoxSizeMode.CenterImage;
            }
            else
            {
                if (pictureMarkerSymbol == null)
                    return;
                Esri.ArcGISRuntime.UI.RuntimeImage image = await pictureMarkerSymbol.CreateSwatchAsync(Color.WhiteSmoke);
                System.Windows.Media.ImageSource imageSource = await Esri.ArcGISRuntime.UI.RuntimeImageExtensions.ToImageSourceAsync(image);
                Image result = imageWpfToGDI(imageSource);
                previewBox.Image = result;
                previewBox.SizeMode = PictureBoxSizeMode.AutoSize | PictureBoxSizeMode.CenterImage;
            }
        }

        /// <summary>
        /// 将madia.imageSource转换成Bitmap
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private System.Drawing.Image imageWpfToGDI(System.Windows.Media.ImageSource image)
        {
            MemoryStream ms = new MemoryStream();
            var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(image as System.Windows.Media.Imaging.BitmapSource));
            encoder.Save(ms);
            ms.Flush();
            return System.Drawing.Image.FromStream(ms);
        }

        /// <summary>
        /// 根据某条件筛选出选择的点样式索引
        /// </summary>
        /// <returns>-1代表未查找到</returns>
        private int findSelectIndex(Predicate<SimpleMarkerSymbolStyle> predicate)
        {
            for(int i = 0; i < style.Count; i++)
            {
                if (predicate(style[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion
    }
}
