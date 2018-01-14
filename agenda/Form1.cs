using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace agenda
{
    public partial class Form1 : Form
    {

        
        
        public Form1()
        {

            eventManager = new EventManager();
            InitializeComponent();



        }

        private void quitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void agendaDrawer1_Paint(object sender, PaintEventArgs e)
        {
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            eventManager.addEvent(new Event(agendaDrawer1.ReturnStartTime(), agendaDrawer1.ReturnStopTime(), textBox3.Text, textBox4.Text));
            MessageBox.Show("Evenement créé avec succès !", "My agenda");
            agendaDrawer1.UpdateEManager(eventManager);
            agendaDrawer1.Invalidate();
        }



        public void UpdateTime()
        {
           
            textBox1.Text = agendaDrawer1.ReturnStartTime().ToString("HH:mm");
            textBox2.Text = agendaDrawer1.ReturnStopTime().ToString("HH:mm");
            eventDrawer1.Invalidate();
        }


    }

    public class AgendaDrawer : Panel
    {
        private int dragX = 0;
        private int dragY = 0;
        private int dragWidth = 0;
        private int dragHeight = 0;
        private bool isClick = false;
        private int posClickX = 0;
        private int posClickY = 0;

        private int memX1 = 0;
        private int memX2 = 0;

        DateTime dateStart;
        DateTime dateStop;

        Rectangle sel = new Rectangle();

        EventManager eManager;


        public AgendaDrawer()
        {

            this.Paint += AgendaDrawer_Paint;
            this.MouseDown += AgendaDrawer_MouseDown;
            this.MouseUp += AgendaDrawer_MouseUp;
            this.MouseMove += AgendaDrawer_MouseMove;
            this.DoubleBuffered = true;
            eManager = new EventManager();


        }

        private void AgendaDrawer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isClick)
            {
                sel.Height = e.Y - posClickY;
                sel.Width = e.X - posClickX;
                this.Invalidate();
                
            }
        }

        private void AgendaDrawer_MouseUp(object sender, MouseEventArgs e)
        {
            memX1 = posClickY;
            memX2 =  sel.Height;
            sel.X = 0;
            sel.Y = 0;
            sel.Height = 0;
            sel.Width = 0;
            posClickX = 0;
            posClickY = 0;
            isClick = false;
            



            (this.Parent as Form1).UpdateTime();

            this.Invalidate();
            
        }

        private void AgendaDrawer_MouseDown(object sender, MouseEventArgs e)
        {

            sel.X = e.X;
            sel.Y = e.Y;
            sel.Height = dragHeight;
            sel.Width = dragWidth;
            posClickX = e.X;
            posClickY = e.Y;
            isClick = true;

            this.Invalidate();

        }

        private void AgendaDrawer_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            Font drawFont = new Font("Arial", 8);
            SolidBrush drawBrush = new SolidBrush(Color.LightGray);
            SolidBrush rectBrush = new SolidBrush(Color.LightGreen);
            double heure = 8.0;

            DateTime dateTime = new DateTime(2017, 1, 1, 0, 0, 0);
            dateTime = dateTime.AddHours(heure);

            graphics.FillRectangle(rectBrush, sel);

            for (int i =1; i<=11; i++)
            {
                graphics.DrawString(dateTime.ToString("HH:mm"), drawFont, drawBrush, new Point(15, i * (this.Size.Height / 11) - 6));
                graphics.DrawLine(Pens.LightGray, new Point(17+(int)graphics.MeasureString(dateTime.ToString("HH:mm"),drawFont).Width, i * (this.Size.Height / 11)), new Point(this.Size.Width - (int)graphics.MeasureString(dateTime.ToString("HH:mm"), drawFont).Width, i * (this.Size.Height / 11)));
                dateTime = dateTime.AddHours(1);
            }

            

        }

        public DateTime ReturnStartTime()
        {
            dateStart = new DateTime(2017, 1, 1, 0, 0, 0);

            for (int i = 0; i <= 11; i++)
            {
                 if(i * (this.Size.Height / 11) > memX1 && i+1 * (this.Size.Height / 11) < memX1)
                {
                    if (i != 1) {dateStart = dateStart.AddHours(8+(double)i-2); }
                    
                    break;
                }
            }

            return dateStart;
        }

        public DateTime ReturnStopTime()
        {
            int nbHour = memX2 / (this.Size.Height / 11);
            dateStop = dateStart;
            dateStop = dateStop.AddHours((double)nbHour+1);

            return dateStop;
        }

        public void UpdateEManager(EventManager eventManager)
        {
            eManager = eventManager;
        }
    }

    public class EventDrawer : Panel
    {
       
        private DateTime startTime;
        private DateTime stopTime;

        public EventDrawer()
        {
            this.Paint += EventDrawer_Paint;
            startTime = new DateTime(2017, 1, 1, 0, 0, 0);
        }

        private void EventDrawer_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            

            Font drawFont = new Font("Arial", 8);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            
        }

        public void SetStartTime(DateTime time)
        {
            startTime = time;
        }

        public void SetStopTime(DateTime time)
        {
            stopTime = time;
        }

    }

    public class Event
    {
        private DateTime startTime = new DateTime(2017, 1, 1, 0, 0, 0);
        private DateTime stopTime = new DateTime(2017, 1, 1, 0, 0, 0);
        private String title;
        private String place;

        public Event(DateTime start, DateTime stop, String title, String place)
        {
            startTime = start;
            stopTime = stop;
            this.title = title;
            this.place = place;
        }

        public Event(DateTime start, DateTime stop, String place)
        {
            startTime = start;
            stopTime = stop;
            this.title = "";
            this.place = place;
        }

        public DateTime StartTime { get => startTime; set => startTime = value; }
        public DateTime StopTime { get => stopTime; set => stopTime = value; }
        public string Title { get => title; set => title = value; }
        public string Place { get => place; set => place = value; }
    }

    public class EventManager
    {
        private List<Event> eventList;

        public EventManager()
        {
            eventList = new List<Event>();
        }

        public void addEvent(Event e)
        {
            eventList.Add(e);
        }

        public void removeEvent(String title)
        {
            
        }

        public List<Event> returnEvents()
        {
            return eventList;
        }

        public int eSize()
        {
           return eventList.Count();
        }
    }


}
