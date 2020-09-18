using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetEti.Globals;
using System.Reflection;
using Vishnu.Interchange;
using NetEti.ApplicationControl;

namespace Vishnu.Demos
{
    /// <summary>
    /// Liefert konfigurierbar wechselnde Ergebnisse: null, true, false, Exception.
    /// </summary>
    public class TrueFalseExceptionChecker : INodeChecker
    {
        #region INodeChecker implementation

        /// <summary>
        /// Kann aufgerufen werden, wenn sich der Verarbeitungs-Fortschritt
        /// des Checkers geändert hat, muss aber zumindest aber einmal zum
        /// Schluss der Verarbeitung aufgerufen werden.
        /// </summary>
        public event CommonProgressChangedEventHandler NodeProgressChanged;

        /// <summary>
        /// Rückgabe-Objekt des Checkers
        /// </summary>
        public object ReturnObject
        {
            get
            {
                return this._returnObject;
            }
            set
            {
                this._returnObject = value;
            }
        }

        /// <summary>
        /// Hier wird der Arbeitsprozess ausgeführt (oder beobachtet).
        /// </summary>
        /// <param name="checkerParameters">Durch Pipe ("|") getrennte Parameterliste:
        /// State[:State(...)]     State kann die Werte 'Null', 'True', 'False' und/oder 'Exception' annehmen.
        /// [|Zustandswechselzeit]  Zeit in Sekunden bis der State wechselt (bei mehreren States), Default 10s.
        /// [|ReturnObject(String), Beispiele: "null:true:false:Exception|20|Resultat" oder "true"||0815".</param>
        /// <param name="treeParameters">Für den gesamten Tree gültige Parameter oder null.</param>
        /// <param name="source">Auslösendes TreeEvent oder null.</param>
        /// <returns>True, False oder null</returns>
        public bool? Run(object checkerParameters, TreeParameters treeParameters, TreeEvent source)
        {
            string pString = (checkerParameters ?? "").ToString().Trim();
            string[] paraStrings = pString.Split('|');
            string[] stateStrings = paraStrings[0].Trim().Split(':');
            List<State> stateList = new List<State>(stateStrings.Select(i => { State x; if (Enum.TryParse(i.Trim().ToUpper(),
                                    out x)) return x; return State.INVALID; }).ToList());
            stateList.RemoveAll(state => state == State.INVALID);
            if (stateList.Count < 1)
            {
                throw new ArgumentException(
                    "Es muss mindestens ein State ('Null','True','False','Exception') oder eine durch ':' getrennte Kombination von States angegeben werden!");
            }
            if (paraStrings.Length > 1)
            {
                string timeString = paraStrings[1].Trim() == "" ? "10" : paraStrings[1].Trim();
                int logicalChangeSeconds;
                if (Int32.TryParse(timeString, out logicalChangeSeconds))
                {
                    this._logicalChangeSeconds = logicalChangeSeconds;
                }
                else
                {
                    this._logicalChangeSeconds = 0;
                }
            }
            if (paraStrings.Length > 2)
            {
                this._userReturnObject = new DefaultReturnObject(paraStrings[2]);
            }
            if (paraStrings.Length > 3)
            {
                string waitTimeString = paraStrings[3].Trim() == "" ? "100" : paraStrings[3].Trim();
                int waiTimeMilliseconds;
                if (Int32.TryParse(waitTimeString, out waiTimeMilliseconds))
                {
                    this._waitTimeMilliseconds = waiTimeMilliseconds;
                }
                else
                {
                    this._waitTimeMilliseconds = 100;
                }
            }
            int actSecond = DateTime.Now.Second;
            if (this._nextIndex < 0 || this._logicalChangeSeconds == 0 || Math.Abs(actSecond - this._lastSecond) >= this._logicalChangeSeconds)
            {
                this._nextDummyReturn = this.getNextDummyReturn(ref this._nextIndex, stateList.ToArray());
                this._lastSecond = actSecond;
            }
            bool? dummyReturn = null;
            if (this._userReturnObject != null)
            {
                this._returnObject = this._userReturnObject;
            }
            else
            {
                this._returnObject = this._nextDummyReturn.ToString();
            }
            switch (this._nextDummyReturn)
            {
                case State.EXCEPTION:
                    ApplicationException exp = new ApplicationException("TestException!");
                    this.ReturnObject = exp;
                    break;
                case State.NULL: dummyReturn = null; break;
                case State.TRUE: dummyReturn = true; break;
                default: dummyReturn = false; break;
            }
            try
            {
                InfoController.Say("#TrueFalseExceptionCecker#: running   0%");
                this.OnNodeProgressChanged(this.GetType().Name, 100, 0, ItemsTypes.items);
                Thread.Sleep(this._waitTimeMilliseconds);
                InfoController.Say("#TrueFalseExceptionCecker#: running  25%");
                this.OnNodeProgressChanged(this.GetType().Name, 100, 25, ItemsTypes.items);
                Thread.Sleep(this._waitTimeMilliseconds);
                InfoController.Say("#TrueFalseExceptionCecker#: running  50%");
                this.OnNodeProgressChanged(this.GetType().Name, 100, 50, ItemsTypes.items);
                Thread.Sleep(this._waitTimeMilliseconds);
                InfoController.Say("#TrueFalseExceptionCecker#: running  75%");
                this.OnNodeProgressChanged(this.GetType().Name, 100, 75, ItemsTypes.items);
                Thread.Sleep(this._waitTimeMilliseconds);
                this.OnNodeProgressChanged(Assembly.GetExecutingAssembly().GetName().Name, 100, 100, ItemsTypes.items);
                if (this._nextDummyReturn == State.EXCEPTION)
                {
                    throw (this.ReturnObject as ApplicationException);
                }
            }
            catch (Exception ex)
            {
                InfoController.Say("#TrueFalseExceptionCecker#: Exception: " + ex.Message);
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(500);
                    InfoController.Say(String.Format($"#TrueFalseExceptionCecker#: hanging {(i + 1) * 10}%"));
                }
                throw;
            }
            //if (this.ReturnObject != null)
            //{
            //  this.ReturnObject = dummyReturn;
            //}
            InfoController.Say("#TrueFalseExceptionCecker#: done    100%");
            return dummyReturn;
        }

        /// <summary>
        /// Schaltet ringförmig durch die verschiedenen Zustände von legalStateSequence.
        /// </summary>
        /// <param name="lastIndex">Der letzte Index in legalStateSequence.</param>
        /// <param name="legalStateSequence">Array von zugelassenen States in Verarbeitungs-Reihenfolge.</param>
        private State getNextDummyReturn(ref int lastIndex, State[] legalStateSequence)
        {
            int nextIndex = 0;
            if (legalStateSequence.Length > 1)
            {
                nextIndex = lastIndex + 1 < legalStateSequence.Length ? lastIndex + 1 : 0;
            }
            lastIndex = nextIndex;
            return legalStateSequence[nextIndex];
        }

        #endregion INodeChecker implementation

        /// <summary>
        /// Standard Konstruktor.
        /// </summary>
        public TrueFalseExceptionChecker()
        {
            this._logicalChangeSeconds = 10;
            this._returnObject = null;
            this._nextDummyReturn = State.INVALID;
            this._nextIndex = -1;
            this._lastSecond = DateTime.Now.Second;
        }

        private State _nextDummyReturn;
        private int _nextIndex;
        private object _returnObject;
        private object _userReturnObject;
        private int _lastSecond;
        private int _logicalChangeSeconds;
        private int _waitTimeMilliseconds;

        private void OnNodeProgressChanged(string itemsName, int countAll, int countSucceeded, ItemsTypes itemsType)
        {
            if (NodeProgressChanged != null)
            {
                NodeProgressChanged(null, new CommonProgressChangedEventArgs(itemsName, countAll, countSucceeded, itemsType, null));
            }
        }
    }

    /// <summary>
    /// Checker-Zustand.
    /// </summary>
    public enum State
    {
        /// <summary>Ungültig (intern).</summary>
        INVALID,
        /// <summary>Checker liefert null als Ergebnis.</summary>
        NULL,
        /// <summary>Checker liefert true als Ergebnis.</summary>
        TRUE,
        /// <summary>Checker liefert false als Ergebnis.</summary>
        FALSE,
        /// <summary>Checker wirft eine Exception.</summary>
        EXCEPTION
    }

    class DefaultReturnObject
    {
        public string DefaultResultProperty { get; private set; }

        public DefaultReturnObject(string info)
        {
            this.DefaultResultProperty = info;
        }

        public override string ToString()
        {
            return this.DefaultResultProperty;
        }
    }
}
