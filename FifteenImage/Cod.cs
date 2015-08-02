#region Using
using System;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
#endregion
namespace FifteenImage
{
    //����� ����������� ������� ���������� IDA*  
        internal sealed class IDA : SearchBase
        {
            #region Constructor
            public IDA(int[] start)
            {
                _start = start;//��������� ���������
            }
            #endregion
            #region Variables
            //������ ����������� ��������� ��������� , ���������� ������ 
            //������������ ������ ���� , ����� �� �����
            public int[] _start;
            //���������� �������������� ���-�� ������������ ���������
            public static ulong _countStates;
            //���������� ������� ������� ������ 
            public static int _depthDistributeStep;
            //���������� ����� �������
            DateTime timeStart = DateTime.Now;
            #endregion
            #region Classes
            //��������� ����������� ����������� (��������� ���������)
            struct Move
            {
                public int m_x, m_y;
                public Move(int x, int y)
                {
                    m_x = x;
                    m_y = y;
                }
            };
            //����� ����������� ��������� ������� , 
            //� ������ ������ ������� ����� ���� 15 
            class State
            {
                //������� �������� �����
                public uint[,] m_cells;
                //������� ��������
                public int m_emptyX, m_emptyY;
                //����������� ������� �������� �������� �� ������ ��������� �������
                public bool IsFinal(int x, int y, int value)
                {
                    return x == -1 ||
                        m_cells[x, y] == value &&
                        IsFinal(y == 0 ? (x - 1) : x, y == 0 ?
                        SearchBase.Dimension - 1 : (y - 1), --value);
                }
                //�������� ������� �������� �������� �� ������ ��������� �������
                /*
                public bool IsFinal()
                {
                    int i, j, start;
                    start = AlgBase.Dimension * AlgBase.Dimension - 2;
                    for (i = AlgBase.Dimension - 1; i >= 0; --i)
                        for (j = i == 3 ? AlgBase.Dimension - 2 : AlgBase.Dimension - 1; j >= 0; --j)
                            if (m_cells[i, j] != start--)
                                return false;
                    return true;
                }
                */
                //�������� �������� �� ������ ��������� �������
                public bool IsFinalPosition()
                {
                    return IsFinal(SearchBase.Dimension - 1, SearchBase.Dimension - 2, SearchBase.Dimension * SearchBase.Dimension - 2);
                }
                //������� ��������������� �������� ������ �� � �����������
                public void SetCell(int x, int y, int value)
                {
                    m_cells[x, y] = (uint)(value - 1);
                }
                //������� ��������������� ��������� ��������
                public void SetEmpty(int x, int y)
                {
                    m_emptyX = x;
                    m_emptyY = y;
                }
                //������� �����������
                public uint[,] Move(Move move)
                {
                    m_cells[m_emptyX, m_emptyY] = m_cells[move.m_x, move.m_y];
                    m_cells[move.m_x, move.m_y] = (uint)(SearchBase.Dimension * SearchBase.Dimension);
                    m_emptyX = move.m_x;
                    m_emptyY = move.m_y;
                    return m_cells;
                }
                //�����������
                public State()
                {
                    m_cells = new uint[SearchBase.Dimension, SearchBase.Dimension];
                    //m_cells = new uint[AlgBase.Dimension][];
                    //for (int i = 0; i < AlgBase.Dimension; i++)
                    //    m_cells[i] = new uint[AlgBase.Dimension];
                }
            };
           class SearchIDA
            {
                //���������� ��� �������� ������� ���������
                public State _state = new State();
                //������ ����������� �� ���������� ��������� � ������� , 
                //�.�. ������� ������� ��������� �����
                public List<Move> _solution = new List<Move>();
                //���������� ������� ������������ ��� ������ �� ���������� ���������� 
                //���� ���������� ������� �� ������� ����� �� ��� �� ���� 
                //�� ����� �������� ������ ����������� 
                int _nStepDepth;
                #region Functions
                /// <summary>
                /// �������� ������� ������ ������������ ����
                /// </summary>
                /// <returns>List<Move> ��������� ������ ������������� ������������ ����</returns>
                public List<Move> GetOptimalSolution()
                {
                    _depthDistributeStep = 0;
                    if (SearchBase.Dimension - 1 == _state.m_emptyX &&
                        _state.m_emptyX == _state.m_emptyY &&
                        _state.IsFinalPosition())//���� ��������� ��������� �������� �����
                        return new List<Move>();//�� ����� ������ ������ ����� ����
                    //������������ ��� ��������� ��������� � ����� ���������� ��������� , 
                    //� ���������� ������� ������ ��� ������ �� ���������� ��������� 
                    _nStepDepth = 0;
                    int depthDistributeStep = 0;
                    //����� ����� 
                    if (!DistributeStep(_state.m_emptyX, _state.m_emptyY, 0, 0, true, depthDistributeStep))
                        //�� ������ � ��������� ���������  , ����� ����� ������ � 
                        //������� ������������� ������� _nStepDepth + 1 
                        while (!DistributeStep(_state.m_emptyX,
                            _state.m_emptyY, 0, 0, false, depthDistributeStep))
                            //����� ����������� ������� ���� �� ������ �� ���������� ����������
                            _nStepDepth++;
                    return _solution;
                }
                /// <summary>
                /// ������������ ��������� ���� , � ������ ���� 15 �� 
                /// ����� ������ ��������� �����������
                /// ������� ��������� ��� ����������� ������� ������
                ///   0 1 2 3
                ///   - - - - Y
                /// 0|
                /// 1|
                /// 2|
                /// 3|
                ///  X
                /// </summary>
                /// <param name="emptyX">��������� �������� �� ��� X</param>
                /// <param name="emptyY">��������� �������� �� ��� Y</param>
                /// <param name="emptyXOffset">����������� �������� ����� ��� X</param>
                /// <param name="emptyYOffset">����������� �������� ����� ��� Y</param>
                /// <param name="edge">false - �������� ����� � ��������� ��������</param>
                /// <returns>true - ���������� ��� �������� ���� � �������  , false �� ��������</returns>
                bool DistributeStep(int emptyX, int emptyY,
                    int emptyXOffset, int emptyYOffset, bool edge, int depthDistributeStep)
                {
                    depthDistributeStep++;
                    if (depthDistributeStep > _depthDistributeStep)
                        _depthDistributeStep = depthDistributeStep;
                    //� ���� "15" �������� 4 �������� ����������� ����� �� ������ �����
                    //���� ����������� �������� ������ �������� 
                    if (emptyY != SearchBase.Dimension - 1 &&
                             -1 != emptyYOffset && //� ���� �� �� ������ ���������� �����������
                             DoMakeStep(emptyX, emptyY, 0, 1, edge, depthDistributeStep)) //�� ������� ��� 
                    {
                        //������ ����������� ������� � ���� ��� �������� 
                        //������������ ������� � ���� , 
                        //�.�. �������� ����� �� ����� ���������� ���� , ������� 
                        //���� ��� � ������ ������  
                        _solution.Add(new Move(emptyX, emptyY + 1));
                        return true;
                    }
                    else if (emptyY != 0 && //���� ����������� �������� ����� �������� 
                             1 != emptyYOffset && //� ���� �� �� ������ ���������� �����������
                             DoMakeStep(emptyX, emptyY, 0, -1, edge, depthDistributeStep))//�� ������� ���
                    {
                        _solution.Add(new Move(emptyX, emptyY - 1));
                        return true;
                    }
                    else if (emptyX != SearchBase.Dimension - 1 && //���� ����������� �������� ���� �������� 
                            -1 != emptyXOffset && //� ���� �� �� ������ ���������� �����������
                             DoMakeStep(emptyX, emptyY, 1, 0, edge, depthDistributeStep))//�� ������� ���
                    {
                        _solution.Add(new Move(emptyX + 1, emptyY));
                        return true;
                    }
                    else if (emptyX != 0 && //���� ����������� �������� ����� �������� 
                            1 != emptyXOffset && //� ���� �� �� ������ ���������� �����������
                             DoMakeStep(emptyX, emptyY, -1, 0, edge, depthDistributeStep))//�� ������� ���
                    {
                        _solution.Add(new Move(emptyX - 1, emptyY));
                        return true;
                    }
                    else //��� 4 ����������� ��� ���������� ��� �������� ��������� 
                        return false;
                }
                /// <summary>
                /// ��������� �� �������� �� ��������� ������ ������ �����������
                /// </summary>
                /// <param name="cell">���������� ������ � ����� ��������� ��������</param>
                /// <param name="emptyX">��������� �������� �� ��� X</param>
                /// <param name="emptyY">��������� �������� �� ��� Y</param>
                /// <param name="emptyXOffset">����������� �������� ����� ��� X</param>
                /// <param name="emptyYOffset">����������� �������� ����� ��� Y</param>
                /// <returns></returns>
                static bool IsGoodMove(uint cell //���������� ������ � ����� ��������� ��������
                    , int emptyX, int emptyY, int emptyXOffset, int emptyYOffset)
                {

                    if (emptyXOffset != 0) //�� ���������� ����������� �� ��� X
                    {
                        int posMustBe = (int)(cell / SearchBase.Dimension);//������� ������� ������
                        //������ ����� ��� ������� ����� ��� ����������� �� ����� 
                        //�������� (��� ��� ����������� �������� �� ����� ������)
                        if (posMustBe == emptyX)
                            return true;//��������� ������
                        else
                            if (emptyX < posMustBe)//���� ������� ������� ������ ��������� ������
                                //�� ����� ������ ���� �� ���������� ����������� 
                                //�������� ���� (=> ������ �����) , �.�. �� ��������� � ����
                                return emptyXOffset < 0;
                            else//����� ���� ������� ������� ������ ��������� �����
                                //�� ����� ������ ���� �� ���������� ����������� 
                                //�������� ����� (=> ������ ����), �.�. �� ��������� � ����
                                return emptyXOffset > 0;
                    }
                    else //�� ���������� ����������� �� ��� Y
                    {
                        int posMustBe = (int)(cell % SearchBase.Dimension);//������� ������� ������
                        //������ ����� ��� ������� ����� ��� ����������� �� ����� �������� 
                        //(��� ��� ����������� �������� �� ����� ������)
                        if (posMustBe == emptyY)
                            return true;//��������� ������
                        else
                            if (emptyY < posMustBe)//���� ������� ������� ������ ��������� ������
                                //�� ����� ������ ���� �� ���������� ����������� �������� ����� 
                                //(=> ������ ������) , �.�. �� ��������� � ����
                                return emptyYOffset < 0;
                            else//����� ���� ������� ������� ��������� �����
                                //�� ����� ������ ���� �� ���������� ����������� �������� ������ 
                                //(=> ������ �����) , �.�. �� ��������� � ����
                                return emptyYOffset > 0;
                    }
                }
                /// <summary>
                /// ��������� ���
                /// </summary>
                /// <param name="emptyX">��������� �������� �� ��� X</param>
                /// <param name="emptyY">��������� �������� �� ��� Y</param>
                /// <param name="emptyXOffset">����������� �������� ����� ��� X</param>
                /// <param name="emptyYOffset">����������� �������� ����� ��� Y</param>
                /// <param name="edge">false - �������� ����� � ��������� ��������</param>
                /// <returns>true - ��� �������� ���� � �������  , false �� ��������</returns>
                bool DoMakeStep(int emptyX, int emptyY, int emptyXOffset,
                    int emptyYOffset, bool edge, int depthDistributeStep)
                {
                    //����� ��������� ��������
                    int newEmptyX = emptyX + emptyXOffset;
                    int newEmptyY = emptyY + emptyYOffset;
                    //���������� ������ � ����� ��������� ��������
                    uint cell = _state.m_cells[newEmptyX, newEmptyY];
                    //�������� �� ������ ����������� �������� ��������� ������
                    bool isGoodMove = IsGoodMove(cell, emptyX, emptyY, emptyXOffset, emptyYOffset);
                    if (isGoodMove)
                    {
                        _countStates++;
                        //���� ����������� �������� �� �������� ��������� ������ �� ���������� �����������
                        _state.m_cells[emptyX, emptyY] = cell;
                        if (edge &&//�� ��������� � ��������� ������ � ������������� ����������� 
                            SearchBase.Dimension - 1 == newEmptyX &&//���� �������� �� ����� �� ��� X
                            SearchBase.Dimension - 1 == newEmptyY &&//���� �������� �� ����� �� ��� Y
                            _state.IsFinalPosition())//���� ����� ��������� ������ �������� � ���� 
                        {
                            //�� ��������� true - ���� �������� 
                            return true;
                        }
                        //���� ����������� �������� �������� ��������� ������ ,
                        //�� ��������� �������� ����������� ��� ������ ��������� ��������
                        else if (DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                            emptyYOffset, edge, depthDistributeStep))
                        {
                            //������ ����������� ������� � ���� ��� �������� ������������ ������� ���� 
                            //� ���� , �.�. �������� ����� �� ����� ���������� ���� , ����� ������  
                            return true;
                        }
                        //���� ��� 4 ��������� ����������� �������� ��������� �� ���������� ����� 
                        //��������� �������� � ������� � false
                        else
                        {
                            _state.m_cells[newEmptyX, newEmptyY] = cell;
                            return false;
                        }
                    }
                    //��������� �� �� � ��������� ������ ������ �� ���������� ���������� 
                    else if (!edge)
                    {
                        _countStates++;
                        //������ ����������� �������� ,�� �� ��������� ��� 
                        //�������� ������� ����������� 
                        //������� ������ ������ �����������
                        _state.m_cells[emptyX, emptyY] = cell;
                        --_nStepDepth;//��������� ����� ����� ����������� , 
                        //� ������ ,���� �� �� ����������� ,
                        //�� ����� ������� ��� ��������� ����������� ��� ������ 
                        //�� ���������� ���������� 
                        if (_nStepDepth >= 0
                            ? DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                            emptyYOffset, false, depthDistributeStep)
                            : DistributeStep(newEmptyX, newEmptyY, emptyXOffset,
                            emptyYOffset, true, depthDistributeStep))
                        {
                            //������ ����������� ������� � ���� ��� �������� 
                            //������������ ������� ���� 
                            //� ���� , �.�. �������� ����� �� ����� 
                            //���������� ���� , ����� ������  
                            return true;
                        }
                        //��������� ������� ������ ������ �� ���������� ���������� 
                        //�� ���� ���������� , �������� ������� 
                        ++_nStepDepth;
                        //���������� ����� ��������� �������� � ������� � false
                        _state.m_cells[newEmptyX, newEmptyY] = cell;
                        return false;
                    }
                    else //�� �� ��������� � ��������� ������ �� ���������� ����������,
                        //� ������ ����������� �������� ��������� 
                        return false;
                }
                #endregion
            };
            #endregion
            #region Functions
            //������ 0.2 ��� ������� ������� ���������� ������ ���������
            protected override void ShowStatistics()
            {
                while (thread != null && thread.IsAlive)//(arResult == null)
                {
                    TimeSpan timeSearch = DateTime.Now - timeStart;
                    Progressing(
                        "������� ���-�� ������������� ���������\t: " + _countStates.ToString(CultureInfo.InvariantCulture) +
                        Environment.NewLine + "����� ������\t\t\t\t: " + timeSearch.ToString() +
                        Environment.NewLine + "������� ������������ ������� ������ \t: " + _depthDistributeStep.ToString(CultureInfo.InvariantCulture));
                    Thread.Sleep(200);
                }
            }
            //�������� ����� �� ������ ��������� ��������� ���� ���������� � ��������
            //������ �������� ���-�� ����������� � ������� ����� �������
            public static bool DoHaveResolve(int[] ar)
            {
                int ch = 0;
                for (int i = 0; i < ar.Length; i++)
                    for (int j = 0; j < ar.Length; j++)
                    {
                        //�������� ����� ��� �������� �����������
                        if (ar[i] == ar.Length - 1 || ar[j] == ar.Length - 1)
                            continue;
                        if (i < j && ar[i] > ar[j])
                            ch++;
                    }
                //���� ������� ������� ������ , ���� ������ ��������� �����
                if (SearchBase.Dimension % 2 == 0)
                    for (int i = 0; i < ar.Length; i++)
                        if (ar[i] == ar.Length - 1)
                        {
                            int row = 0;
                            for (int h = i; h >= 0; h -= SearchBase.Dimension) row++;
                            if (row % 2 == 1)
                                ch++;
                        }
                return (ch % 2) == 0;
            }
            //�������� ������� ����������� ����� 
            protected override void StartSearch()
            {
                Started();
               /* if (!DoHaveResolve(_start))
                    Progressing("������� �� �����. ������� �� ������");
                else*/
                {
                    SearchIDA searchIDA = new SearchIDA();
                    State state = new State();
                    for (int i = 0; i < SearchBase.Dimension * SearchBase.Dimension; ++i)
                    {
                        int value = _start[i];
                        value++;
                        if (value != SearchBase.Dimension * SearchBase.Dimension)
                            state.SetCell(i / SearchBase.Dimension, i % SearchBase.Dimension, value);
                        else
                            state.SetEmpty(i / SearchBase.Dimension, i % SearchBase.Dimension);
                    }
                    searchIDA._state.m_cells = (uint[,])state.m_cells.Clone();
                    searchIDA._state.m_emptyX = state.m_emptyX;
                    searchIDA._state.m_emptyY = state.m_emptyY;
                    _countStates = 0;
                    //����� ����� ������������ ����
                    List<Move> solution = searchIDA.GetOptimalSolution();
                    //�� �������� ����� ����������� �� ������� ������� � ���������
                    solution.Reverse();
                    arResult = new int[solution.Count + 1][];
                    arResult[0] = _start;
                    int arIndex = 1;
                    foreach (Move m in solution)
                    {
                        state.Move(m);
                        arResult[arIndex] = new int[SearchBase.Dimension * SearchBase.Dimension];
                        int j = 0;
                        for (int ii = 0; ii < SearchBase.Dimension; ii++)
                            for (int jj = 0; jj < SearchBase.Dimension; jj++, j++)
                            {
                                if (ii == state.m_emptyX &&
                                    jj == state.m_emptyY)
                                    arResult[arIndex][j] = SearchBase.Dimension * SearchBase.Dimension - 1;
                                else
                                    arResult[arIndex][j] = (int)state.m_cells[ii, jj];// - 1;
                            }
                        arIndex++;
                    }
                    TimeSpan timeSearch = DateTime.Now - timeStart;
                    if (arResult.Length - 1 != 0)
                    {
                        int v=(arResult.Length - 1)%10;
                        string st = (v == 2 || v == 3 || v == 4) ? " ����" : ((v == 1) ? " ���" : " �����");
                        Progressing("����",
                            "�����. " + Environment.NewLine + (arResult.Length).ToString(CultureInfo.InvariantCulture) +
                            st+Environment.NewLine + "�����:" + timeSearch.ToString() +
                            Environment.NewLine + "\n����� ���-�� ������������� ���������\t: " +
                            _countStates.ToString(CultureInfo.InvariantCulture) +
                            Environment.NewLine + "\n**** ����� �������� ****" +
                            Environment.NewLine + "\n����� ������\t: " + timeSearch.ToString() +
                            Environment.NewLine + "\n���-�� ����� ��� ����� �������\t: " +
                            (arResult.Length ).ToString(CultureInfo.InvariantCulture) +
                            Environment.NewLine + "\n  �� ������ ������ ������ \n(��� Ctrl+P) � ��������� ������� ����������� �����.");
                    }
                    else
                        Progressing("� ��� � ������ ������ ?");
                }
                Finished();
            }
            #endregion
        }

     
}