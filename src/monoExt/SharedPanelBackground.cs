using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clayui;
using UnityEngine;
using UnityEngine.UI;
using foundation;

namespace clayui
{
    public class SharedPanelBackground : MonoBehaviour
    {
        private ClayButton _btn_close;
        public ClayButton btn_close
        {
            get
            {
                if (_btn_close == null)
                {
                    _btn_close = new ClayButton(btn_close_skin);
                }
                return _btn_close;
            }
        }

        public GameObject btn_close_skin;
        public Text titleText;
        public string title
        {
            set
            {
                if (titleText != null)
                {
                    titleText.text = value;
                }
            }
        }
    }
}
