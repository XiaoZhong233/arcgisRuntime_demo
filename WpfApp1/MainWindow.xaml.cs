using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Offline;
using Esri.ArcGISRuntime.UI;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using WpfApp1.Enum;
using WpfApp1.form;
using WpfApp1.ViewModel;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        #region 私有变量
        private static Boolean debug = false;
        //保管结点列表
        private Dictionary<string, PropertyNodeItem> layersMap = new Dictionary<string, PropertyNodeItem>();
        private List<String> fileNames = new List<string>();
        //图层树数据源
        List<PropertyNodeItem> itemList = new List<PropertyNodeItem>();
        //当前激活的底图菜单项
        private System.Windows.Controls.MenuItem curBaseMapMenuItem;
        //记录当前的绘制状态
        private OperationType curOperationType = OperationType.无;
        //记录当前的绘制图层
        private GraphicsOverlay graphicLayer;
        //记录当前绘制的线
        private Esri.ArcGISRuntime.Geometry.PointCollection linePoints = new Esri.ArcGISRuntime.Geometry.PointCollection(new SpatialReference(3857));//记录当前绘制的折线点
        //private List<Graphic> temLineList = new List<Graphic>();//记录当前绘制的线段缓存
        //private List<Graphic> linesList = new List<Graphic>();//记录总的折线数
        //记录当前绘制的面
        private Esri.ArcGISRuntime.Geometry.PointCollection areaPoints = new Esri.ArcGISRuntime.Geometry.PointCollection(new SpatialReference(3857));//记录当前绘制的多边形点
        //private List<Graphic> areasList = new List<Graphic>(); //记录总的多边形数
        //private List<Graphic> temAreaList = new List<Graphic>(); //记录当前绘制的多边形缓存
        

        private bool isCaptured = false;//右键捕获标志
        private SimpleMarkerSymbol vertexSymbol;//顶点的符号样式
        private GraphicsOverlay vertexLayer; //顶点图形层
        private Graphic curSelGraphic;//当前选中图形对象
        private MapPoint orgPoint;//鼠标移动时的位移计算原点
        private MyList<Graphic> listOfClipGraphics; //剪切操作时选择的图形
        private MyList<Graphic> listOfBufGraphics; //缓冲区选择要素
        private Graphic cutter; //分割要素
        private Graphic cuttedGraphic;//被分割的要素

        private LocalServerManager localServer;

        private Geodatabase myDatbase;
        private IReadOnlyList<GeodatabaseFeatureTable> myTables;
        public static MainWindow mainwindow;

        #endregion

        #region 属性
        public OperationType CurOperationType
        {
            get { return curOperationType; }
            set
            {
                //当修改有效时时raise
                if (value != curOperationType)
                {
                    WhenValueStateChange(curOperationType, value);
                }

                curOperationType = value;
                //只要属性被修改，就会raise
                WhenValueStateSet();
                
                
            }
        }



        #endregion

        #region 委托与事件
        //用于监听操作状态的修改
        private delegate void OperationTypeSet(object sender, EventArgs e);
        private event OperationTypeSet onOperationTypeSet;
        //用于监听操作状态的改变
        private delegate void OperationTypeChange(object sender, OperationType oldState,OperationType newState);
        private event OperationTypeChange onOperationTypeChange;
        //回调触发
        private void WhenValueStateSet()
        {
            onOperationTypeSet?.Invoke(this, null);
        }
        private void WhenValueStateChange(OperationType oldState, OperationType newState)
        {
            onOperationTypeChange?.Invoke(this, oldState, newState);
        }


        #endregion

        #region 初始化
        public MainWindow()
        {
            InitializeComponent();
            //初始化底图
            initMap();
            initParam();
            
        }

        /// <summary>
        /// 初始化一些参数
        /// </summary>
        private void initParam()
        {
            CurOperationType = OperationType.无;
            vertexSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Square,
                    System.Drawing.Color.Black, 8.0);
            curSelGraphic = null;
            orgPoint = null;
            listOfClipGraphics = new MyList<Graphic>();
            listOfBufGraphics = new MyList<Graphic>();
            listOfClipGraphics.Capacity = 2;
            //监听容器的大小变化
            listOfClipGraphics.onAdd += (o, graphic) =>
            {
                if (listOfClipGraphics.Count >= 2)
                {
                    complete_btn.Visibility = Visibility.Visible;
                }
                else
                {
                    complete_btn.Visibility = Visibility.Hidden;
                }
            };
            listOfClipGraphics.onClear += (o, e) =>
            {
                complete_btn.Visibility = Visibility.Hidden;
            };
            listOfBufGraphics.onAdd += (o, grahic) =>
            {
                if (listOfBufGraphics.Count > 0)
                {
                    complete_btn.Visibility = Visibility.Visible;
                }
            };
            listOfBufGraphics.onRemove += (o, graphic) =>
            {
                if (listOfBufGraphics.Count >= 1)
                {
                    complete_btn.Visibility = Visibility.Visible;
                }
                else
                {
                    complete_btn.Visibility = Visibility.Hidden;
                }
            };
            listOfBufGraphics.onClear += (o, e) =>
            {
                complete_btn.Visibility = Visibility.Hidden;
            };

            localServer = new LocalServerManager(this);
        }

        /// <summary>
        /// 初始化graphiLayer
        /// </summary>
        private void initGraphiLayer()
        {
            graphicLayer = new GraphicsOverlay();
            MyMapView.GraphicsOverlays.Add(graphicLayer);
            graphicLayer.SelectionColor = System.Drawing.Color.Red;
            graphicLayer.Graphics.CollectionChanged += (s, e) =>
            {
                NotifyCollectionChangedEventArgs args = e as NotifyCollectionChangedEventArgs;
                if (args != null)
                {
                    //列表为空
                    if (args.NewStartingIndex == -1)
                    {
                        EditVertexMenuItem.IsEnabled = false;
                        StopEditMenuItem.IsEnabled = false;
                    }
                    else
                    {
                        EditVertexMenuItem.IsEnabled = true;
                        StopEditMenuItem.IsEnabled = false;
                    }
                }
            };
        }

        /// <summary>
        /// 初始化顶点图层
        /// </summary>
        private void initVertexLayer()
        {
            vertexLayer = new GraphicsOverlay();
            MyMapView.GraphicsOverlays.Add(vertexLayer);
            vertexLayer.SelectionColor = System.Drawing.Color.Red;
            vertexLayer.Graphics.CollectionChanged += (s, e) =>
            {
                NotifyCollectionChangedEventArgs args = e as NotifyCollectionChangedEventArgs;
                if (args != null)
                {
                    //列表为空
                    if (args.NewStartingIndex == -1)
                    {
                        EditVertexMenuItem.IsEnabled = false;
                        StopEditMenuItem.IsEnabled = false;
                    }
                    else
                    {
                        EditVertexMenuItem.IsEnabled = true;
                        StopEditMenuItem.IsEnabled = false;
                    }
                }
            };
        }

        //初始化地图处理
        private void initMap()
        {
            //MyMapView.Map.Basemap = null;
            //MyMapView.BackgroundGrid.IsVisible = false;
            // 新建底图
            Map myMap = new Map(basemapOptions.Values.First());
            //新建鹰眼底图（与底图保持一致）
            eagleMapView.Map = new Map(eagleBasemapOptions.Values.First());
            //把网格去掉，改变背景
            MyMapView.BackgroundGrid.IsVisible = false;
            eagleMapView.BackgroundGrid.IsVisible = false;

            // 向mapView中加入底图
            MyMapView.Map = myMap;

            //设置底图的视点
            MyMapView.SetViewpoint(new Viewpoint(23.15019, 113.337059, 15));

            myDatbase = null;
            myTables = null;

            // use a static method on Map to create a map from a web map url
            //异步加载,不阻塞当前进程
            //Uri uri = new Uri(@"http://www.arcgis.com/home/webmap/viewer.html?webmap=e574fe5f2611479681288d1939d8f93f");
            //Map webMap = await Map.LoadFromUriAsync(new Uri("http://www.arcgis.com/home/webmap/viewer.html?webmap=e574fe5f2611479681288d1939d8f93f"));
            //MyMapView.Map = await Map.LoadFromUriAsync(uri);

            //BasemapChooser.ItemsSource = basemapOptions.Keys;
            //BasemapChooser.SelectedIndex = 0;

            // 加入检测加载状态的委托给事件
            myMap.LoadStatusChanged += OnMapsLoadStatusChanged;
            curBaseMapMenuItem = FindName("b") as System.Windows.Controls.MenuItem;

            //订阅左键绘制
            MyMapView.MouseLeftButtonDown += onMapViewLeftMouseButtonDownAsync;

            //订阅双击事件
            MyMapView.MouseDoubleClick += onMapViewMouseDoubleClick;

            //订阅右键按下事件
            MyMapView.MouseLeftButtonDown += onMapViewLeftMouseDown;

            //订阅右键松开事件
            MyMapView.MouseLeftButtonUp += onMapViewLeftMouseUp;

            //订阅鼠标移动事件
            MyMapView.MouseMove += onMapViewMouseMove;

            //监听操作属性变换
            onOperationTypeSet += (s, e) =>
            {
                operationStatus.Text = CurOperationType.ToString();
            };

            operationStatus.Text = curOperationType.ToString();

            onOperationTypeChange += (s,o,n) =>
            {
                //说明此时用户已经操作过一些东西了
                if(complete_btn.Visibility == Visibility.Visible)
                {
                    //onCompleteItemClick(s, new EventArgs());
                    linePoints.Clear();
                    areaPoints.Clear();
                    complete_btn.Visibility = Visibility.Hidden;
                    graphicLayer.ClearSelection();

                    //结束编辑顶点
                    if (o == OperationType.编辑顶点)
                    {
                        stopEditVertex();
                    }
                }
                

            };
        }

        #endregion

        #region 私有方法
        //鹰眼探测区域包围盒
        private Envelope eagleEnvelope;


        //检测加载状况回调方法
        private void OnMapsLoadStatusChanged(object sender, LoadStatusEventArgs e)
        {

         

            Dispatcher.BeginInvoke(
                new ThreadStart(() =>
                {

                    LoadStatusLabel.Content = string.Format("Map's load status : {0}", e.Status.ToString());
                    LoadStatusLabel.Visibility = Visibility.Visible;
                    switch (e.Status.ToString())
                    {
                        case "Loading":
                            LoadStatusLabel.Content = string.Format("加载中");
                            operationStatus.Text = string.Format("加载中");
                            break;
                        case "Loaded":
                            LoadStatusLabel.Content = string.Format("加载完成");
                            operationStatus.Text = string.Format(OperationType.无.ToString());
                            LoadStatusLabel.Visibility = Visibility.Hidden;
                            //加载成功时的操作
                            initGraphiLayer();
                            initVertexLayer();
                            mainwindow = this;
                            break;
                    }
                }

                ));
        }




        // 底图的选项表
        private readonly Dictionary<string, Basemap> basemapOptions = new Dictionary<string, Basemap>()
        {
            
            {"Streets (Raster)", Basemap.CreateStreets()},
            {"空白底图",null },
            {"Streets (Vector)", Basemap.CreateStreetsVector()},
            {"Streets - Night (Vector)", Basemap.CreateStreetsNightVector()},
            {"Imagery (Raster)", Basemap.CreateImagery()},
            {"Imagery with Labels (Raster)", Basemap.CreateImageryWithLabels()},
            {"Imagery with Labels (Vector)", Basemap.CreateImageryWithLabelsVector()},
            {"Dark Gray Canvas (Vector)", Basemap.CreateDarkGrayCanvasVector()},
            {"Light Gray Canvas (Raster)", Basemap.CreateLightGrayCanvas()},
            {"Light Gray Canvas (Vector)", Basemap.CreateLightGrayCanvasVector()},
            {"Navigation (Vector)", Basemap.CreateNavigationVector()},
            {"OpenStreetMap (Raster)", Basemap.CreateOpenStreetMap()}
        };

        /// <summary>
        /// 鹰眼图的底图选项表
        /// </summary>
        private readonly Dictionary<string, Basemap> eagleBasemapOptions = new Dictionary<string, Basemap>()
        {
            
            {"Streets (Raster)", Basemap.CreateStreets()},
            {"空白底图",null },
            {"Streets (Vector)", Basemap.CreateStreetsVector()},
            {"Streets - Night (Vector)", Basemap.CreateStreetsNightVector()},
            {"Imagery (Raster)", Basemap.CreateImagery()},
            {"Imagery with Labels (Raster)", Basemap.CreateImageryWithLabels()},
            {"Imagery with Labels (Vector)", Basemap.CreateImageryWithLabelsVector()},
            {"Dark Gray Canvas (Vector)", Basemap.CreateDarkGrayCanvasVector()},
            {"Light Gray Canvas (Raster)", Basemap.CreateLightGrayCanvas()},
            {"Light Gray Canvas (Vector)", Basemap.CreateLightGrayCanvasVector()},
            {"Navigation (Vector)", Basemap.CreateNavigationVector()},
            {"OpenStreetMap (Raster)", Basemap.CreateOpenStreetMap()}
        };

        public object VisualUpwardSeach { get; private set; }

        /// <summary>
        /// 缩放至图层
        /// </summary>
        /// <param name="layer"></param>
        public void zoomToLayer(Layer layer)
        {
            if(layer!=null)
                MyMapView.SetViewpointGeometryAsync(layer.FullExtent);
        }


        /// <summary>
        /// 根据所选的样式画点
        /// </summary>
        /// <param name="location"></param>
        private void drawMapPoint(MapPoint location)
        {
            if (CurOperationType != OperationType.画点)
                return;
            SimpleMarkerSymbol pointSymbol = MyMapView.Tag as SimpleMarkerSymbol;
            if (pointSymbol != null)
            {
                Graphic gPT = new Graphic(location, pointSymbol);
                graphicLayer.Graphics.Add(gPT);
            }
        }


        /// <summary>
        /// 画顶点，画的点保存在顶点图层
        /// </summary>
        /// <param name="location"></param>
        /// <param name="pointSymbol"></param>
        private void drawMapPoint(MapPoint location, SimpleMarkerSymbol pointSymbol)
        {
            
            Graphic gPT = new Graphic(location, pointSymbol);
            vertexLayer.Graphics.Add(gPT);
        }


        /// <summary>
        /// 根据位置画线
        /// </summary>
        /// <param name="location"></param>
        private void drawMapLine(MapPoint location)
        {
            if (CurOperationType != OperationType.画线)
                return;
            SimpleLineSymbol simpleLineSymbol = MyMapView.Tag as SimpleLineSymbol;
            if (simpleLineSymbol == null)
                return;
            linePoints.Add(location);//将当前点加入点集
            //设置点符号
            SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
            simpleMarkerSymbol.Color = System.Drawing.Color.Black;

            //画线
            drawMapLine(linePoints, simpleLineSymbol);

            //画点
            //CurOperationType = DrawType.画点;
            drawMapPoint(location, simpleMarkerSymbol);
            //CurOperationType = DrawType.画线;
        }


        /// <summary>
        /// 根据点集画线，点集数大于2
        /// </summary>
        /// <param name="points"></param>
        /// <param name="simpleLineSymbol"></param>
        private void drawMapLine(Esri.ArcGISRuntime.Geometry.PointCollection points, SimpleLineSymbol simpleLineSymbol)
        {
            if (CurOperationType != OperationType.画线 || points.Count < 2)
                return;
            if (points.Count == 2)
            {
                Polyline line = new Polyline(linePoints);//由点集构造线
                Graphic lineGraphic = new Graphic(line, simpleLineSymbol);//创建图形
                graphicLayer.Graphics.Add(lineGraphic);
                //linesList.Add(lineGraphic);
            }
            else
            {
                //获取当前绘制的线
                Graphic curGraphic = graphicLayer.Graphics.Last();
                PolylineBuilder pb = new PolylineBuilder(points);
                curGraphic.Geometry = pb.ToGeometry();
            }

            //旧方法，改用builder构建线段
            //if (temLineList.Count > 0)
            //{
            //    //获取当前绘制的线
            //    Graphic curGraphic = temLineList.Last();
            //    graphicLayer.Graphics.Remove(curGraphic);
            //}
            //Polyline line = new Polyline(linePoints);//由点集构造线
            //Graphic lineGraphic = new Graphic(line, simpleLineSymbol);//创建图形
            //graphicLayer.Graphics.Add(lineGraphic);
            //temLineList.Add(lineGraphic);//同时，将该图形加入链表
        }


        /// <summary>
        /// 传入位置，画面
        /// </summary>
        /// <param name="location"></param>
        private void drawMapArea(MapPoint location)
        {
            if (CurOperationType != OperationType.画面)
                return;
            areaPoints.Add(location);
          
            //画面
            SimpleFillSymbol simpleFillSymbol = MyMapView.Tag as SimpleFillSymbol;
            if (simpleFillSymbol == null)
                return;

            drawMapArea(areaPoints, simpleFillSymbol);

            //画顶点
            SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
            simpleMarkerSymbol.Color = System.Drawing.Color.Black;
            drawMapPoint(location, simpleMarkerSymbol);
        }

        /// <summary>
        /// 传入点集，画面，点集数大于3
        /// </summary>
        /// <param name="points"></param>
        /// <param name="simpleFillSymbol"></param>
        private void drawMapArea(Esri.ArcGISRuntime.Geometry.PointCollection points, SimpleFillSymbol simpleFillSymbol)
        {
            if (CurOperationType != OperationType.画面 || points.Count < 3)
                return;

            
            if (points.Count == 3)
            {
                Polygon area = new Polygon(points);
                Graphic areaGraphics = new Graphic(area, simpleFillSymbol);
                graphicLayer.Graphics.Add(areaGraphics);
                //areasList.Add(areaGraphics);
            }
            else
            {
                //获取当前绘制的多边形
                Graphic curGraphic = graphicLayer.Graphics.Last();
                PolygonBuilder pb = new PolygonBuilder(points);
                curGraphic.Geometry = pb.ToGeometry();
            }



            //旧方法弃用，改用build构建多边形
            //if (temAreaList.Count > 0)
            //{
            //    Graphic curGraphic = temAreaList.Last();
            //    graphicLayer.Graphics.Remove(curGraphic);
            //}
            //Polygon area = new Polygon(points);
            //Graphic areaGraphics = new Graphic(area, simpleFillSymbol);
            //graphicLayer.Graphics.Add(areaGraphics);
            //temAreaList.Add(areaGraphics);
        }

        /// <summary>
        /// 计算点之间的曼哈顿距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public double GetDistanceBetweenPoints(MapPoint p1, MapPoint p2)
        {
            double dis = 0.0;
            dis = Math.Sqrt(Math.Abs((p1.X - p2.X)) + Math.Abs((p1.Y - p2.Y)));
            return dis;
        }

        /// <summary>
        /// 判断包含关系
        /// </summary>
        /// <param name="env"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool IsEnvelopeContains(Envelope env, MapPoint pt)
        {
            if (pt.X < env.XMax && pt.X > env.XMin && pt.Y < env.YMax && pt.Y > env.YMin)
                return true;
            else
                return false;
        }


        /// <summary>
        /// 设置是否可以与地图进行交互
        /// </summary>
        /// <param name="enable"></param>
        private void setInteractionEnable(bool enable)
        {
            MyMapView.InteractionOptions = new MapViewInteractionOptions();
            MyMapView.InteractionOptions.IsEnabled = enable;
        }

        /// <summary>
        /// 在某图层中识别要素
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="screenPoint"></param>
        /// <param name="tolerance"></param>
        /// <param name="selectedColor"></param>
        private async void indentifyAsync(GraphicsOverlay overlay, Point screenPoint, double tolerance, System.Drawing.Color selectedColor)
        {
            overlay.SelectionColor = selectedColor;
            overlay.ClearSelection();
            IdentifyGraphicsOverlayResult ir = await
                        MyMapView.IdentifyGraphicsOverlayAsync(overlay, screenPoint,
                        tolerance, false);
            if (ir.Graphics.Count > 0)
            {
                curSelGraphic = ir.Graphics.First();
                curSelGraphic.IsSelected = true;
            }
        }

        /// <summary>
        /// 在指定图层中识别要素，选中颜色为红色
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="sreenPoint"></param>
        /// <param name="tolerance"></param>
        private void indentifyAsync(GraphicsOverlay overlay, Point sreenPoint, double tolerance)
        {
            indentifyAsync(overlay, sreenPoint, tolerance, System.Drawing.Color.Red);
        }

        /// <summary>
        /// 在指定图层中识别要素，选中颜色为红色,容差为5
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="sreenPoint"></param>
        private void indentifyAsync(GraphicsOverlay overlay, Point sreenPoint)
        {
            indentifyAsync(overlay, sreenPoint, 5);
        }


        /// <summary>
        /// 在某图层中识别要素
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="screenPoint"></param>
        /// <param name="tolerance"></param>
        /// <param name="selectedColor"></param>
        /// <param name="action"></param>
        private async void indentifyAsync(GraphicsOverlay overlay, Point screenPoint, double tolerance, System.Drawing.Color selectedColor,Action<IdentifyGraphicsOverlayResult> action)
        {
            overlay.SelectionColor = selectedColor;
            IdentifyGraphicsOverlayResult ir = await
                       MyMapView.IdentifyGraphicsOverlayAsync(overlay, screenPoint,
                       tolerance, false);

            //do something
            action(ir);
        }

        /// <summary>
        /// 在指定图层中识别要素，选中颜色为红色
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="sreenPoint"></param>
        /// <param name="tolerance"></param>
        /// <param name="action"></param>
        private void indentifyAsync(GraphicsOverlay overlay, Point sreenPoint, double tolerance, Action<IdentifyGraphicsOverlayResult> action)
        {
            indentifyAsync(overlay, sreenPoint, tolerance, System.Drawing.Color.Red, action);
        }

        /// <summary>
        /// 在指定图层中识别要素，选中颜色为红色,容差为5
        /// </summary>
        /// <param name="overlay"></param>
        /// <param name="sreenPoint"></param>
        /// <param name="action"></param>
        private void indentifyAsync(GraphicsOverlay overlay, Point sreenPoint, Action<IdentifyGraphicsOverlayResult> action)
        {
            indentifyAsync(overlay, sreenPoint, 5, action);
        }

        /// <summary>
        /// 开始编辑顶点
        /// </summary>
        private void editVertex()
        {
            CurOperationType = OperationType.编辑顶点;
            EditVertexMenuItem.IsEnabled = false;
            StopEditMenuItem.IsEnabled = true;
        }

        /// <summary>
        /// 结束编辑顶点
        /// </summary>
        private void stopEditVertex()
        {
            StopEditMenuItem.IsEnabled = false;
            CurOperationType = OperationType.无;
            if (vertexLayer.Graphics.Count > 0)
            {
                vertexLayer.ClearSelection();
                EditVertexMenuItem.IsEnabled = true;
            }
        }

        /// <summary>
        /// 结束某个操作
        /// </summary>
        private void completeOperation()
        {
            switch (CurOperationType)
            {
                case OperationType.画点:
                case OperationType.画线:
                case OperationType.画面:
                    CurOperationType = OperationType.无;
                    linePoints.Clear();
                    areaPoints.Clear();
                    complete_btn.Visibility = Visibility.Hidden;
                    graphicLayer.ClearSelection();
                    break;
                case OperationType.编辑顶点:
                    CurOperationType = OperationType.无;
                    stopEditVertex();
                    break;
                case OperationType.选择:
                    CurOperationType = OperationType.无;
                    graphicLayer.ClearSelection();
                    break;
                case OperationType.裁剪:
                    CurOperationType = OperationType.无;
                    graphicLayer.ClearSelection();
                    clipGeomerty();
                    complete_btn.Visibility = Visibility.Hidden;
                    listOfClipGraphics.Clear();
                    break;
                case OperationType.缓冲区:
                    CurOperationType = OperationType.无;
                    generateBuf();
                    complete_btn.Visibility = Visibility.Hidden;
                    listOfBufGraphics.Clear();
                    break;
                case OperationType.分割:
                    CurOperationType = OperationType.无;
                    complete_btn.Visibility = Visibility.Hidden;
                    cutGeometry(cuttedGraphic, cutter);
                    graphicLayer.ClearSelection();
                    cutter = null;
                    cuttedGraphic = null;
                    break;
            }
        }


        /// <summary>
        /// 生成缓冲区
        /// </summary>
        private void generateBuf()
        {
            if (listOfBufGraphics.Count > 0)
            {
                string result=Interaction.InputBox("输入缓冲区半径(单位：km)", "标题", "1000", -1, -1);
                if (result == "")// do nothing;
                    return;
                double radius = 0;
                if (!Double.TryParse(result,out radius))
                {
                    System.Windows.MessageBox.Show("请输入合法数字");
                    generateBuf();
                }
                radius *= 1000;
                SimpleFillSymbol sf = new SimpleFillSymbol();
                sf.Color = System.Drawing.Color.FromArgb(100, System.Drawing.Color.Aqua);
                foreach (Graphic g in listOfBufGraphics)
                {
                    Esri.ArcGISRuntime.Geometry.Geometry resultGeometry = GeometryEngine.Buffer(g.Geometry, radius);
                    Graphic gg = new Graphic(resultGeometry, sf);
                    graphicLayer.Graphics.Add(gg);
                }
            }
        }


        /// <summary>
        /// 裁剪要素
        /// </summary>
        private void clipGeomerty()
        {
            //暂时只支持两个多边形要素进行裁剪
            if (listOfClipGraphics.Count == 2)
            {
                Graphic g1 = listOfClipGraphics[0];
                Graphic g2 = listOfClipGraphics[1];
                Esri.ArcGISRuntime.Geometry.Geometry resultGeometry = GeometryEngine.Clip(g1.Geometry, g2.Geometry.Extent);
                //更新图形层
                graphicLayer.Graphics.Remove(g1);
                graphicLayer.Graphics.Remove(g2);
                Graphic clipedGraphic = new Graphic(resultGeometry, new SimpleFillSymbol());
                graphicLayer.Graphics.Add(clipedGraphic);
                //更新顶点层
                Polygon pg1 = g1.Geometry as Polygon;
                Polygon pg2 = g2.Geometry as Polygon;
                //需要移除的顶点集合
                List<Graphic> removeList = new List<Graphic>();
                foreach (Graphic p in vertexLayer.Graphics)
                {
                    MapPoint vertex = p.Geometry as MapPoint;
                    bool found = false;
                    foreach(MapPoint p1 in pg1.Parts[0].Points)
                    {
                        if (vertex.IsEqual(p1))
                        {
                            removeList.Add(p);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        continue;
                    foreach (MapPoint p2 in pg2.Parts[0].Points)
                    {
                        if (vertex.IsEqual(p2))
                        {
                            removeList.Add(p);
                            found = true;
                            break;
                        }
                    }
                }
                //移除顶点
                foreach(Graphic g in removeList)
                {
                    vertexLayer.Graphics.Remove(g);
                }
                //更新顶点
                Polygon polygon = resultGeometry as Polygon;
                SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
                simpleMarkerSymbol.Color = System.Drawing.Color.Black;
                foreach (MapPoint p in polygon.Parts[0].Points)
                {
                    Graphic gpt = new Graphic(p, simpleMarkerSymbol);
                    vertexLayer.Graphics.Add(gpt);
                }
            }
        }

        /// <summary>
        /// 分割要素
        /// </summary>
        private void cutGeometry(Graphic cutted,Graphic cutter)
        {
            if(cutted!=null && cutter != null)
            {
                if(!(cutter.Geometry is Polyline))
                {
                    return;
                }
                Esri.ArcGISRuntime.Geometry.Geometry[] resultGeometry =
                    GeometryEngine.Cut(cutted.Geometry, cutter.Geometry as Polyline);
                //更新图形层
                graphicLayer.Graphics.Remove(cutted);
                foreach(Esri.ArcGISRuntime.Geometry.Geometry g in resultGeometry)
                {
                    switch (g.GeometryType)
                    {
                        case GeometryType.Point:
                            SimpleMarkerSymbol pointSymbol = new SimpleMarkerSymbol();
                            Graphic pt = new Graphic(g, pointSymbol);
                            graphicLayer.Graphics.Add(pt);
                            break;
                        case GeometryType.Polyline:
                            Graphic pl = new Graphic(g, new SimpleLineSymbol());
                            graphicLayer.Graphics.Add(pl);
                            break;
                        case GeometryType.Polygon:
                            Graphic pg = new Graphic(g, new SimpleFillSymbol());
                            graphicLayer.Graphics.Add(pg);
                            break;
                    }
                };
                //更新顶点层
                //需要移除的顶点集合
                List<Graphic> removeList = new List<Graphic>();
                if(cutted.Geometry.GeometryType == GeometryType.Point || cutted.Geometry.GeometryType == GeometryType.Multipoint)
                {
                    //点要素没有顶点
                    return;
                }
                removeList.AddRange(removeVetexByGeometry(cutted));
                //移除顶点
                foreach (Graphic g in removeList)
                {
                    vertexLayer.Graphics.Remove(g);
                }
                //更新顶点
                SimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
                simpleMarkerSymbol.Color = System.Drawing.Color.Black;
                foreach (Esri.ArcGISRuntime.Geometry.Geometry g in resultGeometry)
                {
                    switch (g.GeometryType)
                    {
                        case GeometryType.Polygon:
                            Polygon polygon = g as Polygon;

                            foreach (MapPoint p in polygon.Parts[0].Points)
                            {
                                Graphic gpt = new Graphic(p, simpleMarkerSymbol);
                                vertexLayer.Graphics.Add(gpt);
                            }
                            break;
                        case GeometryType.Polyline:
                            Polyline polyline = g as Polyline;
                            foreach (MapPoint p in polyline.Parts[0].Points)
                            {
                                Graphic gpt = new Graphic(p, simpleMarkerSymbol);
                                vertexLayer.Graphics.Add(gpt);
                            }
                            break;
                    }
                }
            }
            else
            {
                return;
            }
        }


        /// <summary>
        /// 移除特定图形的顶点
        /// </summary>
        private List<Graphic> removeVetexByGeometry(Graphic graphic)
        {
            List<Graphic> removeList = new List<Graphic>();
            if (graphic == null)
                return removeList;
            if(graphic.Geometry.GeometryType == GeometryType.Polyline)
            {
                Polyline pl = graphic.Geometry as Polyline;
                foreach (Graphic p in vertexLayer.Graphics)
                {
                    MapPoint vertex = p.Geometry as MapPoint;
                    foreach (MapPoint p1 in pl.Parts[0].Points)
                    {
                        if (vertex.IsEqual(p1))
                        {
                            removeList.Add(p);
                            break;
                        }
                    }
                }
            }else if(graphic.Geometry.GeometryType == GeometryType.Polygon)
            {
                Polygon pg = graphic.Geometry as Polygon;
                foreach (Graphic p in vertexLayer.Graphics)
                {
                    MapPoint vertex = p.Geometry as MapPoint;
                    foreach (MapPoint p1 in pg.Parts[0].Points)
                    {
                        if (vertex.IsEqual(p1))
                        {
                            removeList.Add(p);
                            break;
                        }
                    }
                }
            }
            return removeList;
        }

        #endregion

        #region 事件处理
        /// <summary>
        /// 视点改变回调，在这里设置鹰眼地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myView_viewPointChange(object sender, EventArgs e)
        {
            //每次调用,刷新鹰眼图
            eagleMapView.GraphicsOverlays.Clear();
            //获取当前的范围的多边形
            Esri.ArcGISRuntime.Geometry.Polygon polygon = MyMapView.VisibleArea;
            //获取当前范围
            eagleEnvelope = polygon.Extent;
            //鹰眼框的颜色
            System.Drawing.Color lineColor = System.Drawing.Color.FromName("Red");
            //边框样式
            Esri.ArcGISRuntime.Symbology.SimpleLineSymbol lineSymbol = new Esri.ArcGISRuntime.Symbology.SimpleLineSymbol(Esri.ArcGISRuntime.Symbology.SimpleLineSymbolStyle.Dash, lineColor, 3);
            //鹰眼框的面填充A为0代表透明面
            System.Drawing.Color fillColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            //获得鹰眼多边形样式
            Esri.ArcGISRuntime.Symbology.SimpleFillSymbol polySymbol = new Esri.ArcGISRuntime.Symbology.SimpleFillSymbol(Esri.ArcGISRuntime.Symbology.SimpleFillSymbolStyle.Solid, fillColor, lineSymbol);
            //获取底图的图层
            var overlay = new Esri.ArcGISRuntime.UI.GraphicsOverlay();
            //获取鹰眼图层，用刚创建的面图例圈起来
            var eagleOverlay = new Esri.ArcGISRuntime.UI.Graphic(eagleEnvelope, polySymbol);
            //底图图层叠加获得的鹰眼图层
            overlay.Graphics.Add(eagleOverlay);
            //将整个图层加入MapView中
            eagleMapView.GraphicsOverlays.Add(overlay);
        }

        /// <summary>
        /// 将矢量要素图层加入地图，更新至图层树控件
        /// </summary>
        private async void addFeatureLayerAsync(FeatureLayer featureLayer, string file)
        {
            //打开文件
            MyMapView.Map.OperationalLayers.Add(featureLayer);
            await MyMapView.SetViewpointGeometryAsync(featureLayer.FullExtent);
            //截取文件名
            string directoryName = Path.GetDirectoryName(file);//截取出除去文件类型的路径名
            string fileName = Path.GetFileNameWithoutExtension(file);//文件名
            //更新图层树
            PropertyNodeItem root = null;

            if (layersMap.TryGetValue(directoryName, out root))
            {

                PropertyNodeItem node = new PropertyNodeItem(file, fileName, featureLayer, root);
                node.Icon = @"\Resouces\data_ic.png";
                //如果已经有该文件，直接返回
                if (fileNames.Contains(fileName))
                {
                    //在操作图层中移除该图层，为了和图层树保持一致,不可以有重复的图层
                    MyMapView.Map.OperationalLayers.Remove(featureLayer);
                    return;
                }
                fileNames.Add(fileName);
                //将图层加入到父节点保存的列表中
                List<Layer> layers = root.layers;
                layers.Add(featureLayer);
                //将子节点加入父节点中

                root.Children.Add(node);
            }
            //需要新建父节点
            else
            {
                //新建父节点
                List<Layer> layers = new List<Layer>();
                layers.Add(featureLayer);
                root = new PropertyNodeItem(directoryName, directoryName, layers);
                root.Icon = @"\Resouces\data_ic.png";

                layersMap.Add(directoryName, root);
                //新建子节点
                fileNames.Add(fileName);
                PropertyNodeItem childNode = new PropertyNodeItem(file, fileName, featureLayer, root);
                childNode.Icon = @"\Resouces\data_ic.png";
                //将子节点加入父节点中
                root.Children.Add(childNode);

            }
            if (!itemList.Contains(root))
            {
                itemList.Add(root);
            }

            this.layerTreeView.ItemsSource = itemList;
            layerTreeView.Items.Refresh();
        }


        /// <summary>
        /// 打开文件，仅支持shapefile格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void openFile_click(object sender, RoutedEventArgs e)
        {
            DialogResult result;

            String[] files;
            //Shapefile shapefile = new Shapefile();

            using (OpenFileDialog openDialog = new OpenFileDialog())
            {

                openDialog.Multiselect = true;
                openDialog.InitialDirectory = @"c:/";
                openDialog.Filter = "ShapeFile文件|*.shp;*.mdb";//定义文件筛选器,只显示扩展名为.shp的文件；
                openDialog.Title = "请打开ShapeFile或者GeodataBase文件";
                result = openDialog.ShowDialog();//返回用户选择
                openDialog.RestoreDirectory = true;//关闭前对话框恢复当前目录
                files = openDialog.FileNames;//保存选中文件路径
            }

            if (result == System.Windows.Forms.DialogResult.OK && files != null)
            {

                foreach (String file in files)
                {
                    if (Regex.IsMatch(file, "^.*(.shp)$"))
                    {
                        Debug.Print(file);
                        ShapefileFeatureTable myShapefile = await ShapefileFeatureTable.OpenAsync(file);
                        FeatureLayer featureLayer = new FeatureLayer(myShapefile);
                        
                        if (MyMapView != null)
                        {
                            addFeatureLayerAsync(featureLayer, file);
                            ////打开文件
                            //MyMapView.Map.OperationalLayers.Add(featureLayer);
                            //await MyMapView.SetViewpointGeometryAsync(featureLayer.FullExtent);

                            //string directoryName = Path.GetDirectoryName(file);//截取出除去文件类型的路径名
                            //string fileName = Path.GetFileNameWithoutExtension(file);//文件名

                            ////更新图层树

                            //PropertyNodeItem root = null;
                            //if(layersMap.TryGetValue(directoryName,out root))
                            //{

                            //    PropertyNodeItem node = new PropertyNodeItem(file, fileName,featureLayer,root);
                            //    node.Icon = @"\Resouces\data_ic.png";
                            //    //如果已经有该文件，直接返回
                            //    if (fileNames.Contains(fileName))
                            //    {
                            //        //在操作图层中移除该图层，为了和图层树保持一致,不可以有重复的图层
                            //        MyMapView.Map.OperationalLayers.Remove(featureLayer);
                            //        return;
                            //    }
                            //    fileNames.Add(fileName);
                            //    //将图层加入到父节点保存的列表中
                            //    List<Layer> layers = root.layers;
                            //    layers.Add(featureLayer);
                            //    //将子节点加入父节点中

                            //    root.Children.Add(node);
                            //}
                            ////需要新建父节点
                            //else
                            //{
                            //    //新建父节点
                            //    List<Layer> layers = new List<Layer>();
                            //    layers.Add(featureLayer);
                            //    root = new PropertyNodeItem(directoryName, directoryName,layers);
                            //    root.Icon = @"\Resouces\data_ic.png";

                            //    layersMap.Add(directoryName, root);
                            //    //新建子节点
                            //    fileNames.Add(fileName);
                            //    PropertyNodeItem childNode = new PropertyNodeItem(file, fileName, featureLayer,root);
                            //    childNode.Icon = @"\Resouces\data_ic.png";
                            //    //将子节点加入父节点中
                            //    root.Children.Add(childNode);

                            //}
                            //if (!itemList.Contains(root))
                            //{
                            //    itemList.Add(root);
                            //}

                            //this.layerTreeView.ItemsSource = itemList;
                            //layerTreeView.Items.Refresh();


                        }
                    }
                    else if (Regex.IsMatch(file, "^.*(.mdb)$"))
                    {
                        Debug.Print(file);

                        Geodatabase mobileGeodatabase = await Geodatabase.OpenAsync(file);
                        GeodatabaseFeatureTable featureTable = mobileGeodatabase.GeodatabaseFeatureTable("terlk_p");
                        await featureTable.LoadAsync();
                        FeatureLayer trailheadsFeatureLayer = new FeatureLayer(featureTable);
                        MyMapView.Map.OperationalLayers.Add(trailheadsFeatureLayer);
                        await MyMapView.SetViewpointGeometryAsync(trailheadsFeatureLayer.FullExtent);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("匹配失败！");
                    }
                }
            }
            else
            {
                
                System.Windows.MessageBox.Show("无结果");
            }

        }

        /// <summary>
        /// 打开GeoDataBase文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void openLocalGeoDBAsync(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openDlg = new
                    System.Windows.Forms.OpenFileDialog();
            openDlg.Filter = "GeoDatabase File(*.geodatabase)|*.geodatabase";
            openDlg.Title = "Open GeoDatabase File";
            if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = openDlg.FileName;
                try
                {

                    myDatbase = await Geodatabase.OpenAsync(path);//打开数据库 
                    myTables = myDatbase.GeodatabaseFeatureTables;//获取所有表对象
                    //遍历所有表对象
                    foreach(GeodatabaseFeatureTable t in myTables)
                    {
                        await t.LoadAsync();
                        FeatureLayer ly = new FeatureLayer(t);//构造要素图层 
                        //MyMapView.Map.OperationalLayers.Add(ly); //添加图层到地图控件中
                        addFeatureLayerAsync(ly, path);
                        OpenGDB.IsEnabled = false;
                    }
                    //myDatbase.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 属性查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryByAtb(object sender, RoutedEventArgs e)
        {
            QueryForm qf = new QueryForm(myTables);
            qf.ShowDialog();
        }


        private QueryGeoType curQueryGeoType = QueryGeoType.无;
        /// <summary>
        /// 空间查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queryByLoc(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            CurOperationType = OperationType.查询;
            switch (item.Name)
            {
                case "queryByPoint":
                    curQueryGeoType = QueryGeoType.点;       
                    break;
                case "queryByRec":
                    curQueryGeoType = QueryGeoType.矩形;
                    break;
            }
        }

        /// <summary>
        /// 显示自定义在线地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void openOnlineMap(object sender, RoutedEventArgs e)
        {

            Map webMap1 = await Map.LoadFromUriAsync(new Uri("http://www.arcgis.com/home/webmap/viewer.html?webmap=e574fe5f2611479681288d1939d8f93f"));

            webMap1.LoadStatusChanged += OnMapsLoadStatusChanged;

            MyMapView.Map = webMap1;

            Map webMap2 = await Map.LoadFromUriAsync(new Uri("http://www.arcgis.com/home/webmap/viewer.html?webmap=e574fe5f2611479681288d1939d8f93f"));

            eagleMapView.Map = webMap2;
        }


        #region 图层树事件处理
        /// <summary>
        /// 图层树节点右键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                PropertyNodeItem dataContext = treeViewItem.DataContext as PropertyNodeItem;
                if (dataContext != null)
                {
                    if (debug)
                    {
                        System.Windows.MessageBox.Show("右击 "+dataContext.DisplayName + "");
                    }
                    treeViewItem.Focus();
                    e.Handled = true;
                }
                
            }
        }

        /// <summary>
        /// 获得图层树的结点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }


        /// <summary>
        /// 图层树的结点勾选事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onChecked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                PropertyNodeItem dataContext = treeViewItem.DataContext as PropertyNodeItem;
                if (dataContext != null )
                {
                    if (debug)
                    {
                        System.Windows.MessageBox.Show("复选框状态 " + checkBox.IsChecked + " " + "节点名：" + dataContext.DisplayName);
                        System.Windows.MessageBox.Show(dataContext.layer.CanChangeVisibility+"");
                    }
                    //如果是子节点
                    if (dataContext.nodeType == PropertyNodeItem.NodeType.LeafNode)
                    {
                       
                        FeatureLayer featureLayer = dataContext.layer as FeatureLayer;
                        if (featureLayer != null)
                        {
                            featureLayer.IsVisible = true;
                        }
                    }
                    //父节点
                    else
                    {
                        //遍历父节点下的子节点并对其进行操作
                        PropertyNodeItem.traveseNode(dataContext, x =>
                        {
                            FeatureLayer featureLayer = x.layer as FeatureLayer;
                            if (featureLayer != null)
                            {
                                featureLayer.IsVisible = true;
                            }
                        });
                    }
                    
                    treeViewItem.Focus();
                    e.Handled = true;

                }
            }
        }

        private void onUnChecked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                PropertyNodeItem dataContext = treeViewItem.DataContext as PropertyNodeItem;
                if (dataContext != null)
                {
                    if (debug)
                    {
                        System.Windows.MessageBox.Show("复选框状态 " + checkBox.IsChecked + " " + "节点名：" + dataContext.DisplayName);
                    }
                    treeViewItem.Focus();
                    e.Handled = true;
                   
                    //如果是子节点
                    if(dataContext.nodeType == PropertyNodeItem.NodeType.LeafNode)
                    {
                        FeatureLayer featureLayer = dataContext.layer as FeatureLayer;
                        if (featureLayer != null)
                        {
                            featureLayer.IsVisible = false;
                        }
                    }
                    //如果是父节点
                    else
                    {
                        //遍历父节点下的子节点并对其进行操作
                        PropertyNodeItem.traveseNode(dataContext, x =>
                        {
                            FeatureLayer featureLayer = x.layer as FeatureLayer;
                            if (featureLayer != null)
                            {
                                featureLayer.IsVisible = false;
                            }
                        });
                    }
                }
            }
        }

        private void removeLayer(PropertyNodeItem layerNode)
        {
            //如果是子节点
            if (layerNode.nodeType == PropertyNodeItem.NodeType.LeafNode)
            {
                //如果当前子节点的父节点只有一个子节点，则直接移除父节点
                if(layerNode.parent!=null && layerNode.parent.Children.Count == 1)
                {
                    //在操作图层中移除
                    MyMapView.Map.OperationalLayers.Remove(layerNode.layer);
                    //把当前节点保存路径名的map进行移除
                    fileNames.Remove(layerNode.DisplayName);
                    //把当前节点的父节点保存的目录名对应的map进行移除
                    layersMap.Remove(layerNode.parent.DisplayName);
                    //treeView数据源移除
                    itemList.Remove(layerNode.parent);

                }
                else
                {
                    //移除当前节点与父节点的关系
                    layerNode.parent.Children.Remove(layerNode);
                    layerNode.parent.layers.Remove(layerNode.layer);
                    //把当前节点关联的图层移除
                    MyMapView.Map.OperationalLayers.Remove(layerNode.layer);
                    //把当前节点保存路径名的map进行移除
                    fileNames.Remove(layerNode.DisplayName);
                    //treeView数据源移除
                    //itemList.Remove(layerNode);
                }

                
            }
            //如果是父节点
            else if(layerNode.nodeType == PropertyNodeItem.NodeType.RootNode)
            {
                //遍历该结点下的所有节点
                PropertyNodeItem.traveseNode(layerNode, x =>
                {
                    //如果是子节点
                    if (x.nodeType == PropertyNodeItem.NodeType.LeafNode)
                    {
                        MyMapView.Map.OperationalLayers.Remove(x.layer);
                        fileNames.Remove(x.DisplayName);
                        //itemList.Remove(x);
                    }
                    else if(x.nodeType == PropertyNodeItem.NodeType.RootNode)
                    {
                        layersMap.Remove(x.DisplayName);
                        itemList.Remove(x);
                    }
                    else
                    {

                    }
                });
            }
            else
            {

            }
            this.layerTreeView.Items.Refresh();

        }

        /// <summary>
        /// 图层树节点右键菜单删除项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onRemoveItemClick(object sender, RoutedEventArgs e)
        {
         
            var dataContext = layerTreeView.SelectedItem as PropertyNodeItem;
            if (dataContext != null)
            {
                
                removeLayer(dataContext);
                if (!debug)
                {
                    int num = 0;
                    if (itemList.Count > 0)
                    {
                        num = itemList.First().Children.Count;
                    }
                    System.Windows.MessageBox.Show(String.Format("被移除结点的名称：{0}\n 当前tv中的数据个数：{1}\n" +
                        "当前保存的目录个数：{2}\n 当前保存的文件路径个数：{3}\n 第一个父节点的子节点数：{4}\n" +
                        "移除的节点的深度：{5}",dataContext.Name,itemList.Count,layersMap.Count,fileNames.Count, num,dataContext.Level));
                }
                
            }
        }

        /// <summary>
        /// 图层树节点右键菜单重命名项点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onRenameItemClick(object sender, RoutedEventArgs e)
        {
            //do something
        }

        /// <summary>
        /// 图层树节点右键菜单查看属性表事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onProperityClick(object sender, RoutedEventArgs e)
        {
            var dataContext = layerTreeView.SelectedItem as PropertyNodeItem;
            if (dataContext != null && dataContext.layer!=null)
            {
                WpfApp1.View.properityForm proForm = new View.properityForm(dataContext.layer);
                proForm.Owner = this;
                proForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                proForm.Show();
            }
        }

        /// <summary>
        /// 图层树节点右键菜单符号项事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onSymbolItemClick(object sender, RoutedEventArgs e)
        {
            var dataContext = layerTreeView.SelectedItem as PropertyNodeItem;
            if (dataContext != null && dataContext.layer != null)
            {
                FeatureLayer shapeFile = dataContext.layer as FeatureLayer;
                switch (shapeFile.FeatureTable.GeometryType)
                {
                    case GeometryType.Point:
                        PointSymbolForm pointForm = new PointSymbolForm(dataContext.layer);
                        pointForm.ShowDialog();
                        break;
                    case GeometryType.Polyline:
                        LineSymbolForm lineForm = new LineSymbolForm(dataContext.layer);
                        lineForm.ShowDialog();
                        break;
                    case GeometryType.Polygon:
                        PolygonSymbolForm polygonSymbolForm = new PolygonSymbolForm(dataContext.layer);
                        polygonSymbolForm.ShowDialog();
                        break;
                    default:
                        break;
                }
            }
        }



        /// <summary>
        /// 图层树节点的双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null )
            {
                PropertyNodeItem dataContext = treeViewItem.DataContext as PropertyNodeItem;
                if (debug)
                {
                    System.Windows.MessageBox.Show("双击"+dataContext.DisplayName + "");
                }
                Layer layer = dataContext.layer;
                if (layer != null)
                {
                    //缩放至该图层
                    MyMapView.SetViewpointGeometryAsync(layer.FullExtent);
                }
            }
            
        }

        #endregion

        /// <summary>
        /// 底图菜单各选项的点击事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBaseMapMenuItemCLick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;
            if (menuItem != null)
            {
                string selectedBasemapTtile = menuItem.Header as string;
                if (selectedBasemapTtile != null)
                {
                    if (debug)
                    {
                        System.Windows.MessageBox.Show("name: " + selectedBasemapTtile);
                       
                    }
                    //取消原先选中的项
                    curBaseMapMenuItem.IsChecked = false;

                    MyMapView.Map.Basemap = basemapOptions[selectedBasemapTtile];
                    eagleMapView.Map.Basemap = eagleBasemapOptions[selectedBasemapTtile];
                    //空白底图的处理
                    if (MyMapView.Map.Basemap == null && eagleMapView.Map.Basemap == null)
                    {
                        MyMapView.BackgroundGrid.IsVisible = false;
                        eagleMapView.BackgroundGrid.IsVisible = false;
                        MyMapView.BackgroundGrid.Color = System.Drawing.Color.White;
                        eagleMapView.BackgroundGrid.Color = System.Drawing.Color.White;
                    }
                    else
                    {
                        MyMapView.BackgroundGrid.IsVisible = true;
                        eagleMapView.BackgroundGrid.IsVisible = true;
                    }
                    curBaseMapMenuItem = menuItem;
                    menuItem.IsChecked = true;
                    
                }
            }
        }


    


        #region 绘制菜单事件处理

        private void onDrawPointItemClick(object sender, RoutedEventArgs e)
        {
            //如果没有则需要加入到mapView中
            if (graphicLayer == null)
            {
                initGraphiLayer();
            }
            PointSymbolForm form = new PointSymbolForm();
            form.ShowDialog();
            CurOperationType = OperationType.画点;
            MyMapView.Tag = form.SimpleMarkerSymbol;

        }

        private void onDrawLineItemClick(object sender, RoutedEventArgs e)
        {
            //如果没有则需要加入到mapView中
            if (graphicLayer == null)
            {
                initGraphiLayer();
            }
            LineSymbolForm form = new LineSymbolForm();
            form.ShowDialog();
            CurOperationType = OperationType.画线;
            MyMapView.Tag = form.SimpleLineSymbol;
        }

        private void onDrawAreaItemClick(object sender, RoutedEventArgs e)
        {
            //如果没有则需要加入到mapView中
            if (graphicLayer == null)
            {
                initGraphiLayer();
            }
            PolygonSymbolForm form = new PolygonSymbolForm();
            form.ShowDialog();
            CurOperationType = OperationType.画面;
            MyMapView.Tag = form.SimpleFillSymbol;
        }

        private void onClearDrawItemClick(object sender, RoutedEventArgs e)
        {
            //System.Windows.MessageBox.Show("线段数： " + linesList.Count + "\n" + "多边形数: " + linesList.Count);
            if (graphicLayer != null)
            {
                graphicLayer.Graphics.Clear();
                MyMapView.GraphicsOverlays.Remove(graphicLayer);
                graphicLayer = null;
            }
            if (vertexLayer!=null)
            {
                vertexLayer.Graphics.Clear();
            }
            CurOperationType = OperationType.无;
            //temLineList.Clear();
            //linesList.Clear();
            linePoints.Clear();
            //areasList.Clear();
            areaPoints.Clear();
        }

        #endregion


        /// <summary>
        /// 完成按钮点击事件点击处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCompleteItemClick(object sender,EventArgs e)
        {
            completeOperation();
        }




        /// <summary>
        /// mapView左键绘制处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task onMapViewLeftMouseButtonDownAsync(object sender,MouseButtonEventArgs e)
        {
            IInputElement mv = sender as IInputElement;
            if (mv != null && graphicLayer!=null)
            {
                MapPoint location = MyMapView.ScreenToLocation(e.GetPosition(mv));
                Point ScreenPos = e.GetPosition(mv);
                switch (CurOperationType)
                {
                    case OperationType.无:
                        //do nothing
                        break;
                    case OperationType.画点:
                        complete_btn.Visibility = Visibility.Visible;
                        drawMapPoint(location);
                        break;
                    case OperationType.画线:
                        complete_btn.Visibility = Visibility.Visible;
                        drawMapLine(location);
                        break;
                    case OperationType.画面:
                        complete_btn.Visibility = Visibility.Visible;
                        drawMapArea(location);
                        break;
                    case OperationType.编辑顶点:
                        complete_btn.Visibility = Visibility.Visible;
                        vertexLayer.ClearSelection();
                        indentifyAsync(vertexLayer, ScreenPos);
                        break;
                    case OperationType.选择:
                        complete_btn.Visibility = Visibility.Visible;
                        graphicLayer.ClearSelection();
                        indentifyAsync(graphicLayer, ScreenPos);
                        break;
                    case OperationType.裁剪:
                        indentifyAsync(graphicLayer, ScreenPos, result =>
                         {
                             

                             if (result.Graphics.Count > 0)
                             {
                                 Graphic sel = result.Graphics.First();
                                 //如果不是多边形，则不选择
                                 if (sel.Geometry.GeometryType != GeometryType.Polygon)
                                 {
                                     return;
                                 }
                                 sel.IsSelected = true;
                                 if (!listOfClipGraphics.Contains(sel))
                                 {
                                     if (listOfClipGraphics.Count == 2)
                                     {
                                         sel.IsSelected = false;
                                         return;
                                     }
                                     listOfClipGraphics.Add(sel);
                                 }
                                 else
                                 {
                                     //取消选中
                                     listOfClipGraphics.Remove(sel);
                                     sel.IsSelected = false;
                                 }
                                    
                             }
                             else
                             {
                                 graphicLayer.ClearSelection();
                                 listOfClipGraphics.Clear();
                             }

                             
                         });
                        break;
                    case OperationType.缓冲区:
                        indentifyAsync(graphicLayer, ScreenPos, result =>
                        {
                            if (result.Graphics.Count > 0)
                            {
                                Graphic sel = result.Graphics.First();
                                if (!listOfBufGraphics.Contains(sel))
                                {
                                    listOfBufGraphics.Add(sel);
                                    sel.IsSelected = true;
                                }
                                else
                                {
                                    sel.IsSelected = false;
                                    listOfBufGraphics.Remove(sel);
                                }
                                
                            }
                            //取消全部选中
                            else
                            {
                                graphicLayer.ClearSelection();
                                listOfBufGraphics.Clear();
                            }
                        });
                        break;
                    case OperationType.分割:
                        indentifyAsync(graphicLayer, ScreenPos, result =>
                        {
                            if (result.Graphics.Count > 0)
                            {
                                Graphic sel = result.Graphics.First();
                                //检查是否点的是自己
                                if(sel == cutter || sel == cuttedGraphic)
                                {
                                    sel.IsSelected = false;
                                    if(sel == cutter)
                                    {
                                        cutter = null;
                                    }
                                    else
                                    {
                                        cuttedGraphic = null;
                                    }
                                    return;
                                }

                                //初次选中
                                if (cuttedGraphic == null)
                                {
                                    cuttedGraphic = sel;
                                    sel.IsSelected = true;
                                    return;
                                }
                                //说明应该选中分割要素了
                                else
                                {
                                    if (sel.Geometry.GeometryType != GeometryType.Polyline)
                                    {
                                        System.Windows.MessageBox.Show("请选择一个线类型的分割要素");
                                        return;
                                    }
                                    cutter = sel;
                                    sel.IsSelected = true;
                                }
                            }
                            //取消选中
                            else
                            {
                                graphicLayer.ClearSelection();
                                cutter = null;
                                cuttedGraphic = null;
                            }
                            //更新按钮状态
                            if(cuttedGraphic!=null && cutter != null)
                            {
                                complete_btn.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                complete_btn.Visibility = Visibility.Hidden;
                            }
                        });
                        break;
                    case OperationType.查询:
                        if (curQueryGeoType == QueryGeoType.点)
                        {
                            IReadOnlyList<IdentifyLayerResult> results = await
                                            MyMapView.IdentifyLayersAsync(ScreenPos, 15, false);
                            if (results != null)
                            {

                            }
                        }
                        break;
                    default:
                        break;
                }
            }

        }


        /// <summary>
        /// 操作菜单项事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onOperationMenuItemCLick(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem item = sender as System.Windows.Controls.MenuItem;
            if (item != null)
            {
                switch (item.Name)
                {
                    case "identify":
                        CurOperationType = OperationType.选择;
                        break;
                    case "EditVertexMenuItem":
                        editVertex();
                        break;
                    case "StopEditMenuItem":
                        stopEditVertex();
                        break;
                    case "buffer":
                        CurOperationType = OperationType.缓冲区;
                        operationStatus.Text = operationStatus.Text + ",请选择要素";
                        break;
                    case "clip":
                        CurOperationType = OperationType.裁剪;
                        operationStatus.Text = operationStatus.Text + ",请选择两个多边形的要素";
                        break;
                    case "cut":
                        CurOperationType = OperationType.分割;
                        operationStatus.Text = operationStatus.Text + ",请依次选择一个被分割要素和分割要素(线类型)";
                        break;
                    case "generalize":
                        break;
                    case "intersections":
                        break;
                    case "simplify":
                        break;
                    case "union":
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// mapView双击事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onMapViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            completeOperation();
        }


        /// <summary>
        /// mapView鼠标左键按下事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onMapViewLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement ie = (IInputElement)(sender);
            MapPoint loc = MyMapView.ScreenToLocation(e.GetPosition(ie));
            if (CurOperationType == OperationType.编辑顶点)
            {
                if(curSelGraphic!=null)
                {
                    //顶点捕捉
                    if(curSelGraphic.Geometry.GeometryType == GeometryType.Point)
                    {
                        MapPoint p = (MapPoint)curSelGraphic.Geometry;
                        double dis = GetDistanceBetweenPoints(p, loc);
                        if (dis < 600)
                            isCaptured = true;
                        else
                            isCaptured = false;
                    }

                    //锁定mapView操作
                    if (isCaptured)
                    {
                        MyMapView.Cursor = System.Windows.Input.Cursors.Cross;
                        operationStatus.Text = "已捕捉,拖动鼠标左键编辑";
                        setInteractionEnable(false);
                    }
                    else
                    {
                        MyMapView.Cursor = System.Windows.Input.Cursors.Arrow;
                        operationStatus.Text = "未成功捕捉,请重试";
                    }

                }
            }else if (CurOperationType == OperationType.选择)
            {
                if (curSelGraphic != null)
                {
                    //点捕捉
                    if (curSelGraphic.Geometry.GeometryType == GeometryType.Point)
                    {
                        MapPoint p = (MapPoint)curSelGraphic.Geometry;
                        double dis = GetDistanceBetweenPoints(p, loc);
                        if (dis < 600)
                            isCaptured = true;
                        else
                            isCaptured = false;
                    }
                    //说明当前为线，面的选择状态
                    else
                    {
                        Envelope env = curSelGraphic.Geometry.Extent;
                        if (IsEnvelopeContains(env, loc))
                            isCaptured = true;
                        else
                            isCaptured = false;
                    }

                    //锁定mapView操作
                    if (isCaptured)
                    {
                        MyMapView.Cursor = System.Windows.Input.Cursors.SizeAll;
                        operationStatus.Text = "已捕捉,拖动鼠标左键编辑";
                        setInteractionEnable(false);
                        orgPoint = loc;
                    }
                    else
                    {
                        MyMapView.Cursor = System.Windows.Input.Cursors.Arrow;
                        operationStatus.Text = "未成功捕捉,请重试";
                        orgPoint = null;
                    }
                }
            }

        }


        /// <summary>
        /// mapView鼠标移动事件处理
        /// 在此处处理编辑图形
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onMapViewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            IInputElement ie = (IInputElement)(sender);
            MapPoint loc = MyMapView.ScreenToLocation(e.GetPosition(ie));
            
            //编辑顶点状态下右键拖动处理
            //顶点编辑
            if (e.LeftButton == MouseButtonState.Pressed && isCaptured && CurOperationType == OperationType.编辑顶点 && curSelGraphic!=null)
            {
                operationStatus.Text = "左键按下,拖动";
                operationStatus.Text = loc.ToString();
                //所选的图形位于顶点图形层
                if(curSelGraphic.GraphicsOverlay == vertexLayer)
                {
                    Graphic selectGraphic = null;
                    MapPoint selectVertex = curSelGraphic.Geometry as MapPoint;
                    //需要获取到该顶点图形对应于grahicslayer图层中的位置，也就是第几个点的问题,因此需要遍历graphics图层中的图形
                    foreach (Graphic graphic in graphicLayer.Graphics)
                    {

                        if (graphic.Geometry.GeometryType == GeometryType.Point)
                        {
                            //顶点编辑状态下，不可能选中点要素,我不把点要素看成顶点
                            continue;
                        }

                        //如果为线,遍历线中的所有顶点,选出所选择的图形
                        if (graphic.Geometry.GeometryType == GeometryType.Polyline)
                        {
                            Esri.ArcGISRuntime.Geometry.Polyline polyline = graphic.Geometry as Polyline;
                            foreach(MapPoint p in polyline.Parts[0].Points)
                            {
                                if (!p.IsEqual(selectVertex))
                                    continue;
                                selectGraphic = graphic;
                                break;
                            }
                        }

                        //如果为多边形，同理
                        if(graphic.Geometry.GeometryType == GeometryType.Polygon)
                        {
                            Polygon polygon = graphic.Geometry as Polygon;
                            foreach(MapPoint p in polygon.Parts[0].Points)
                            {
                                if (!p.IsEqual(selectVertex))
                                    continue;
                                selectGraphic = graphic;
                                break;
                            }
                        }
                        
                    }

                    //开始编辑
                    if (selectGraphic != null)
                    {
                        operationStatus.Text = "新顶点位置:"+loc.ToString();
                        if (selectGraphic.Geometry.GeometryType == GeometryType.Polyline)
                        {
                            Polyline polyline = selectGraphic.Geometry as Polyline;
                            Esri.ArcGISRuntime.Geometry.PointCollection pointCollection =
                                new Esri.ArcGISRuntime.Geometry.PointCollection(polyline.Parts[0].Points, new SpatialReference(3857));
                            
                            if (pointCollection.Contains(selectVertex))
                            {
                                int index = pointCollection.IndexOf(selectVertex);
                                pointCollection.SetPoint(index, loc.X, loc.Y);
                                //更新顶点层
                                selectVertex = pointCollection[index];
                                curSelGraphic.Geometry = selectVertex;
                            }

                            //重新生成线
                            PolylineBuilder pb = new PolylineBuilder(pointCollection);
                            selectGraphic.Geometry = pb.ToGeometry();
                            
                        }
                        else if(selectGraphic.Geometry.GeometryType == GeometryType.Polygon)
                        {
                            Polygon polygon = selectGraphic.Geometry as Polygon;
                            Esri.ArcGISRuntime.Geometry.PointCollection pointCollection =
                                new Esri.ArcGISRuntime.Geometry.PointCollection(polygon.Parts[0].Points, new SpatialReference(3857));
                            if (pointCollection.Contains(selectVertex))
                            {
                                int index = pointCollection.IndexOf(selectVertex);
                                pointCollection.SetPoint(index, loc.X, loc.Y);
                                //更新顶点层
                                selectVertex = pointCollection[index];
                                curSelGraphic.Geometry = selectVertex;
                            }
                            //重新生成多边形
                            PolygonBuilder pb = new PolygonBuilder(pointCollection);
                            selectGraphic.Geometry = pb.ToGeometry();
                        }


                    }
                    //未找到所选择的顶点对应的绘制要素图层
                    else
                    {
                        operationStatus.Text = "未找到所选择的顶点对应的绘制要素图层";
                    }
                    
                }
                
            }//移动图形
            else if(e.LeftButton == MouseButtonState.Pressed && isCaptured && CurOperationType == OperationType.选择 && curSelGraphic != null)
            {
                operationStatus.Text = "左键按下,拖动";
                //计算x位移和y位移
                double xDis = loc.X - orgPoint.X;
                double yDis = loc.Y - orgPoint.Y;
                operationStatus.Text = "位移量:"+xDis+","+yDis;
                if (curSelGraphic.Geometry.GeometryType == GeometryType.Point)
                {
                    MapPointBuilder pb = new MapPointBuilder(loc);
                    curSelGraphic.Geometry = pb.ToGeometry();
                }else if(curSelGraphic.Geometry.GeometryType == GeometryType.Polyline)
                {
                    Polyline pl = curSelGraphic.Geometry as Polyline;
                    Esri.ArcGISRuntime.Geometry.PointCollection pointCollection = 
                        new Esri.ArcGISRuntime.Geometry.PointCollection(pl.Parts[0].Points, new SpatialReference(3857));
                    //遍历顶点，移动图形
                    for(int index = 0; index < pl.Parts[0].PointCount; index++)
                    {
                        //计算新顶点
                        MapPoint oldPoint = pointCollection[index];
                        pointCollection.SetPoint(index, oldPoint.X + xDis, oldPoint.Y + yDis);
                        //更新顶点层中的顶点
                        foreach(Graphic g in vertexLayer.Graphics)
                        {
                            MapPoint vertex = g.Geometry as MapPoint;
                            if (vertex.IsEqual(oldPoint))
                            {
                                g.Geometry = new MapPointBuilder(pointCollection[index]).ToGeometry();
                                break;
                            }
                        }
                        
                    }
                    //更新图形层中的顶点
                    PolylineBuilder lb = new PolylineBuilder(pointCollection);
                    curSelGraphic.Geometry = lb.ToGeometry();
                }else if(curSelGraphic.Geometry.GeometryType == GeometryType.Polygon)
                {
                    Polygon pg = curSelGraphic.Geometry as Polygon;
                    Esri.ArcGISRuntime.Geometry.PointCollection pointCollection =
                        new Esri.ArcGISRuntime.Geometry.PointCollection(pg.Parts[0].Points, new SpatialReference(3857));
                    for(int index = 0; index < pg.Parts[0].PointCount; index++)
                    {
                        //计算新顶点
                        MapPoint oldPoint = pointCollection[index];
                        pointCollection.SetPoint(index, oldPoint.X + xDis, oldPoint.Y + yDis);
                        //更新顶点层中的顶点
                        foreach (Graphic g in vertexLayer.Graphics)
                        {
                            MapPoint vertex = g.Geometry as MapPoint;
                            if (vertex.IsEqual(oldPoint))
                            {
                                g.Geometry = new MapPointBuilder(pointCollection[index]).ToGeometry();
                                break;
                            }
                        }
                    }
                    //更新图形层中的顶点
                    PolygonBuilder pb = new PolygonBuilder(pointCollection);
                    curSelGraphic.Geometry = pb.ToGeometry();
                }

                //更新原点位置
                orgPoint = loc;
            }

           

            
            

            //if (isCaptured)
            //{
            //    IInputElement ie = (IInputElement)(sender);
            //    if (ie != null)
            //    {

            //        MapPoint loc = MyMapView.ScreenToLocation(e.GetPosition(ie));
            //        operationStatus.Text = loc.ToString(); 

            //    }
            //}

        }




        /// <summary>
        /// mapView鼠标左键弹起事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onMapViewLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            
            setInteractionEnable(true);
            if ((CurOperationType == OperationType.编辑顶点 || CurOperationType == OperationType.选择)&&isCaptured )
            {
                isCaptured = false;
                MyMapView.Cursor = System.Windows.Input.Cursors.Arrow;
                operationStatus.Text = "左键弹起.编辑完成";
            }

        }





        #endregion

        #region 拓展方法
        public static byte getAlpha(double opacity)
        {
            return (byte)(opacity * 255);
        }

        #endregion
    }
 

}
