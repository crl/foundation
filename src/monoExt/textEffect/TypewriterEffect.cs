
using System;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;

namespace foundation
{
    [RequireComponent(typeof(Text))]
    public class TypewriterEffect : MonoBehaviour
    {
        static public TypewriterEffect current;

        public int charsPerSecond = 20;

        public bool keepFullDimensions = false;

        public Action onFinished;

        Text mLabel;
        string mFullText = "";
        int mCurrentOffset = 0;
        float mNextChar = 0f;
        bool mReset = true;
        bool mActive = false;

        public bool IsWriteFinish()
        {
            return mCurrentOffset == mFullText.Length;
        }

        public bool IsActive { get { return mActive; } }

        public void ResetToBeginning()
        {
            Finish(false);
            mReset = true;
            mActive = true;
        }

        public void Finish(bool realFinish = true)
        {
            if (mActive)
            {
                mActive = false;

                if (!mReset)
                {
                    mCurrentOffset = mFullText.Length;
                    mLabel.text = mFullText;
                }

                current = this;
                current = null;

                if (onFinished != null && realFinish)
                {
                    onFinished();
                }
            }
        }

        void OnEnable() { mReset = true; mActive = true; }

        void Update()
        {
            if (!mActive) return;

            if (mReset)
            {
                mCurrentOffset = 0;
                mReset = false;
                mLabel = GetComponent<Text>();
                mFullText = mLabel.text;
                mNextChar = 0;
            }

            while (mCurrentOffset < mFullText.Length && mNextChar <= Time.realtimeSinceStartup)
            {
                int lastOffset = mCurrentOffset;
                charsPerSecond = Mathf.Max(1, charsPerSecond);

                ++mCurrentOffset;

                float delay = 1f / charsPerSecond;
                char c = (lastOffset < mFullText.Length) ? mFullText[lastOffset] : '\n';

                if (lastOffset + 1 == mFullText.Length || mFullText[lastOffset + 1] <= ' ')
                {
                    if (c == '.')
                    {
                        if (lastOffset + 2 < mFullText.Length && mFullText[lastOffset + 1] == '.' && mFullText[lastOffset + 2] == '.')
                        {
                            lastOffset += 2;
                        }
                    }
                }

                if (mNextChar == 0f)
                {
                    mNextChar = Time.realtimeSinceStartup + delay;
                }
                else mNextChar += delay;

                mLabel.text = keepFullDimensions ?
                        mFullText.Substring(0, mCurrentOffset) + "[00]" + mFullText.Substring(mCurrentOffset) :
                        mFullText.Substring(0, mCurrentOffset);
            }

            if (mCurrentOffset == mFullText.Length)
            {
                Finish();
            }
        }
}
}