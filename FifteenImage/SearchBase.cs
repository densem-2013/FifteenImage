#region Using
using System;
using System.Threading;

#endregion
namespace FifteenImage
{
    public class SearchEventArgs : EventArgs
        {
            public string Title;
            public string Mes;
            public bool DoAddMes;
            public SearchEventArgs(string title, string mes, bool doAddMes) { Title = title; Mes = mes; DoAddMes = doAddMes; }
            public SearchEventArgs() { }
            public SearchEventArgs(string mes) { Title = String.Empty; Mes = mes; DoAddMes = false;}
            public SearchEventArgs(string title, string mes) { Title = String.Empty; Mes = mes; DoAddMes = false; }
            public SearchEventArgs(string mes, bool doAddMes) { Title = String.Empty; Mes = mes; DoAddMes = doAddMes;  }
        }
        public abstract class SearchBase
        {
            public static int Dimension = 4;//����������� ������� ������
            public static int Quantity = Dimension * Dimension;
            #region Events
            public event EventHandler<SearchEventArgs> AlgStarted;
            public event EventHandler<SearchEventArgs> AlgProgressing;
            public event EventHandler<SearchEventArgs> AlgFinished;
            protected void Started()
            {
                if (AlgStarted != null)
                    AlgStarted(this, new SearchEventArgs());
            }
            protected void Progressing(string mes, bool doAddMes)
            {
                if (AlgProgressing != null)
                    AlgProgressing(this, new SearchEventArgs(mes, doAddMes));
            }
            protected void Progressing(string mes)
            {
                if (AlgProgressing != null)
                    AlgProgressing(this, new SearchEventArgs(mes, false));
            }
            protected void Progressing(string title, string mes)
            {
                if (AlgProgressing != null)
                    AlgProgressing(this, new SearchEventArgs(title, mes, false));
            }
            protected void Finished()
            {
                if (AlgFinished != null)
                    AlgFinished(this, new SearchEventArgs());
            }
            public bool EventStartAdded { get { return AlgStarted != null; } }
            public bool EventProgressAdded { get { return AlgProgressing != null; } }
            public bool EventFinishAdded { get { return AlgFinished != null; } }
            public bool EventsAdded { get { return AlgStarted != null && AlgProgressing != null && AlgFinished != null; } }
            #endregion
            #region Variables
            //��������� � ���� ������� �������� ����� � ���������� �� ��������
            public static int[][] arResult;
            //��������� �� ����� � ������� ����� ������������ ������ ���������
            protected static Thread thread;
            //��������� �� ����� ������� ����� �������� ���������� � ������ ���������
            protected static Thread threadStatistics;
            #endregion
            #region Functions
            //������� ��������� ������� ������ � ������ ����������
            public static string DoStop()
            {
                string mes = "";
                if (thread != null)
                {
                    thread.Abort();
                    threadStatistics.Abort();
                    if (!thread.Join(1000))
                        mes = "�������� ��������� ������";
                    else
                    {
                        mes = "����� ������� ����������";
                        thread = null;
                        threadStatistics = null;
                        
                    }
                }
                if (threadStatistics != null)
                {
                    threadStatistics.Abort();
                    if (!threadStatistics.Join(1000))
                        mes = "�������� ��������� ������";
                    else
                        threadStatistics = null;
                }
                return mes;
            }
            //������� ����������� �������� ����� ������ � 
            //��������������� ����� ��� ������ ���������� � 
            //������ ��������� ������
            public void DoStart()
            {
                try
                {
                    if (thread != null)
                    {
                        if (!thread.Join(10000))
                        {
                            thread.Abort();
                            thread.Join();
                        }
                        thread = null;
                    }
                    arResult = null;
                    thread = new Thread(new ThreadStart(StartSearch));
                    thread.IsBackground = true;
                    thread.Start();
                    //Thread.Sleep(1000);
                    if (threadStatistics != null)
                    {
                        if (!threadStatistics.Join(10000))
                        {
                            threadStatistics.Abort();
                            threadStatistics.Join();
                        }
                        threadStatistics = null;
                    }
                    threadStatistics = null;
                    threadStatistics = new Thread(new ThreadStart(ShowStatistics));
                    threadStatistics.IsBackground = true;
                    threadStatistics.Start();
                }
                catch (Exception ex)
                {
                    Progressing(ex.Message);
                    Finished();
                }
            }
            //������ 0.2 ��� ������� ������� ���������� ������ ���������
            protected abstract void ShowStatistics();
            //�������� ������� ����������� ����� 
            protected abstract void StartSearch();
            #endregion
        }

       
 }