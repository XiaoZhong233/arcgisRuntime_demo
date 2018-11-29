using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WpfApp1.form.Controls
{
    public partial class TransparencyControl : UserControl
    {

        #region 私有变量
        // The backbuffer 
        private Bitmap _backBuffer;
        // 透明度带的颜色
        private Color _color;

        // 拖动时要更新的矩形
        private Rectangle _updateRectangle;

        // 绘图时控件被锁定
        private bool _locked = false;

        private Color _colorOutline = Color.FromKnownColor(KnownColor.ControlDark);
        private Color _colorSelection = Color.FromKnownColor(KnownColor.Blue);
        private Color _colorHandle = Color.LightGray;

        // 灰度值（位置）
        byte _value = 255;

        private const int BAND_HEIGHT = 10;
        private const int BAND_OFFSET = 8;
        private const int HANDLE_WIDTH = 10;
        private const int HANDLE_HEIGHT = 16;
        #endregion


        #region 属性
        /// <summary>
        /// 条带填充颜色
        /// </summary>
        public Color BandColor
        {
            get { return _color; }
            set
            {
                _color = value;
                this.Invalidate_(true);
            }
        }

        /// <summary>
        /// 透明度
        /// </summary>
        public byte Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    _handle.Position = PositionFromValue(_value);
                    textBox1.Text = _value.ToString();
                    this.Invalidate_(true);
                }
            }
        }
        #endregion

        /// <summary>
        /// 保存其中一个有关的句柄信息
        /// </summary>
        private class TranspHandle
        {
            internal bool Selected = false;                     // 是否被选中
            internal RectangleF Rectangle = new RectangleF();   // 绘制后的位置
            internal int Position = 0;                          // 像素位置
        }

        // 控件句柄
        TranspHandle _handle = null;

        // 拖动状态标志
        private bool _draggingIsPerformed = false;

        // 拖动开始位置
        private int _draggingStart = 0;

        #region 事件
        // 更改控件的值的委托
        public delegate void ValueChangedDeleg(object sender, byte value);

        //用户更改控件的值的时事件
        public event ValueChangedDeleg ValueChanged;

        /// <summary>
        /// 发送事件给监听器
        /// </summary>
        protected internal void FireValueChanged(object sender, byte value)
        {
            if (ValueChanged != null)
                ValueChanged(this, value);
        }
        #endregion


        public TransparencyControl()
        {
            _updateRectangle = new Rectangle();
            _handle = new TranspHandle();

            InitializeComponent();

            this.SizeChanged += TransparencyControl_SizeChanged;
        }

        void TransparencyControl_SizeChanged(object sender, EventArgs e)
        {
            textBox1.Left = this.Width - textBox1.Width - 10;

        }

        /// <summary>
        /// 改变大小时触发
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.Width > 0 && this.Height > 0)
            {
                if (_backBuffer != null)
                {
                    _backBuffer.Dispose();
                    _backBuffer = null;
                }
                _backBuffer = new Bitmap(this.Width, this.Height);
                _updateRectangle = new Rectangle(0, 0, this.ClientRectangle.Width - textBox1.Width, BAND_HEIGHT + HANDLE_HEIGHT + 3);
                _handle.Position = PositionFromValue(_value);
            }
        }

        #region Drawing
        /// <summary>
        /// 重绘
        /// </summary>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Graphics gResult = e.Graphics;

            if (!_locked)
            {
                _locked = true;
                Graphics g = Graphics.FromImage(_backBuffer);
                g.Clear(Color.Transparent);

                Rectangle rect;

                int w = PositionFromValue(_value) - BAND_OFFSET;
                if (w > 0)
                {
                    rect = new Rectangle(BAND_OFFSET, 0, w, BAND_HEIGHT);
                    Color clr1 = Color.FromArgb(0, _color);
                    Color clr2 = Color.FromArgb(_value, _color);
                    Brush brush = new LinearGradientBrush(rect, clr1, clr2, LinearGradientMode.Horizontal);
                    g.FillRectangle(brush, rect);
                }

                rect = new Rectangle(BAND_OFFSET, 0, BandWidth, BAND_HEIGHT);
                Pen pen = new Pen(Color.Gray);
                g.DrawRectangle(pen, rect);

                DrawHandle(g);

                _locked = false;
            }

            // 重绘颜色条带
            gResult.DrawImage(_backBuffer, 0, 0);

            if (_draggingIsPerformed)
            {
                gResult.Flush(System.Drawing.Drawing2D.FlushIntention.Sync);
            }
        }

        /// <summary>
        /// 绘制指示条
        /// </summary>
        /// <param name="g">Graphics object to draw upon</param>
        private void DrawHandle(Graphics g)
        {
            PointF[] points = new PointF[5];

            RectangleF r = new RectangleF();
            r.X = 0;
            r.Y = 0;
            r.Width = HANDLE_WIDTH;
            r.Height = HANDLE_HEIGHT;

            points[0].X = r.Width / 2; points[0].Y = 0.0f;
            points[1].X = r.Width; points[1].Y = r.Height / 3;
            points[2].X = r.Width; points[2].Y = r.Height;
            points[3].X = 0; points[3].Y = r.Height;
            points[4].X = 0; points[4].Y = r.Height / 3;

            float x = PositionFromValue(_value) - HANDLE_WIDTH / 2;

            Matrix mtx = new Matrix();
            mtx.Translate(x, BAND_HEIGHT + 2);
            g.Transform = mtx;

            Color color = _handle.Selected ? Color.FromKnownColor(KnownColor.ControlDarkDark) : Color.FromKnownColor(KnownColor.ControlDark);
            Color colorFill = _handle.Selected ? Color.FromKnownColor(KnownColor.Control) : Color.FromKnownColor(KnownColor.ControlLight);

            float width = _handle.Selected ? 1.0f : 1.0f;
            g.FillPolygon(new SolidBrush(colorFill), points);
            g.DrawPolygon(new Pen(color, width), points);
            g.ResetTransform();

            r.Offset(x, BAND_HEIGHT + 2);
            _handle.Rectangle = r;
        }
        #endregion


        #region Position
        /// <summary>
        /// 根据值返回指示位置
        /// </summary>
        private int PositionFromValue(byte value)
        {
            float width = BandWidth;
            float position = (float)BAND_OFFSET + (float)value / 255.0f * width;
            return (int)position;
        }


        public int BandWidth
        {
            get { return this.ClientRectangle.Width - textBox1.Width - 2 * BAND_OFFSET - 10; }
        }

        /// <summary>
        /// 根据位置返回透明度
        /// </summary>
        private byte ValueFromPosition(int position)
        {
            int bandWidth = BandWidth;
            if (position < BAND_OFFSET) position = BAND_OFFSET;
            if (position > bandWidth + BAND_OFFSET) position = bandWidth + BAND_OFFSET;
            return (byte)((float)(position - BAND_OFFSET) / (float)(bandWidth) * 255.0f);
        }
        #endregion


        #region Dragging
        /// <summary>
        ///当点击指示条拖动时触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.Enabled)
            {
                RectangleF rect = _handle.Rectangle;

                if (e.X >= rect.X && e.X <= rect.X + rect.Width &&
                    e.Y >= rect.Y && e.Y <= rect.Y + rect.Height)
                {
                    _draggingIsPerformed = true;
                    _draggingStart = e.X;
                }
            }
        }

        /// <summary>
        /// 不断重绘条带
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.Enabled)
            {
                // 改变位置
                if (_draggingIsPerformed)
                {
                    if (_draggingStart != e.X)
                    {
                        _value = ValueFromPosition(_handle.Position + e.X - _draggingStart);
                        textBox1.Text = _value.ToString();
                        this.Invalidate_(true);
                    }
                }
                else
                {
                    // 突出句柄
                    RectangleF rect = _handle.Rectangle;
                    bool newState = (e.X >= rect.X && e.X <= rect.X + rect.Width &&
                                     e.Y >= rect.Y && e.Y <= rect.Y + rect.Height);

                    if (newState != _handle.Selected)
                    {
                        _handle.Selected = newState;
                        Graphics g = Graphics.FromHwnd(this.Handle);
                        DrawHandle(g);
                    }
                }
            }
        }

        /// <summary>
        ///  完成拖动操作触发
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_draggingIsPerformed)
            {
                _draggingIsPerformed = false;
                byte val = ValueFromPosition(_handle.Position + e.X - _draggingStart);
                _value = val;
                textBox1.Text = _value.ToString();
                _handle.Position = PositionFromValue(_value);
                this.Invalidate_(true);
                FireValueChanged(this, val);
            }
        }
        #endregion


        /// <summary>
        /// 锁定控件
        /// </summary>
        private void Invalidate_(bool UpdateRectangleOnly)
        {
            if (UpdateRectangleOnly)
            {
                this.Invalidate(_updateRectangle);
            }
            else
            {
                this.Invalidate();

            }
        }

        /// <summary>
        /// 按键事件处理，并验证输入合法
        /// </summary>
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                textBox1_Validated(null, null);
            }
        }

        /// <summary>
        ///验证输入合法
        /// </summary>
        private void textBox1_Validated(object sender, EventArgs e)
        {
            byte val;
            if (byte.TryParse(textBox1.Text, out val))
            {
                if (val != _value)
                {
                    _value = val;
                    _handle.Position = PositionFromValue(_value);
                    this.Invalidate_(true);
                    FireValueChanged(this, val);
                }
            }
            else
            {
                // reverting to the previous scale
                textBox1.Text = _value.ToString();
            }
        }

        public double getOpacity()
        {
            return (Value) / 255.0;
        }

       
    }
}
