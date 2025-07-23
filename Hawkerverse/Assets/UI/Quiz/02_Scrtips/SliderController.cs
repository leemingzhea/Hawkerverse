using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lordmin
{
    public class SliderController : MonoBehaviour
    {

        public Text Text_SliderInfo;
        private int TempFontSize;
        // Use this for initialization
        void Start()
        {
            Text_SliderInfo.text = "Q1";
            TempFontSize = Text_SliderInfo.fontSize;
            ExamPaperManager.Instance.Text_QuestionNumber.GetComponent<Outline>().enabled = false;
        }


        public void MoveSlider()
        {
            if (ExamPaperManager.Instance.Slider_Question.value != ExamPaperManager.Instance.Slider_Question.maxValue)
            {
                Text_SliderInfo.text = "Q" + (ExamPaperManager.Instance.Slider_Question.value + 1).ToString();
                if (ExamPaperManager.Instance.IsEnd == false)
                {
                    ExamPaperManager.Instance.Text_QuestionNumber.text = "Q" + (ExamPaperManager.Instance.Slider_Question.value + 1);
                    ExamPaperManager.Instance.Text_QuestionThemaName.text = ExamPaperManager.Instance.ExamThemas[ExamPaperManager.Instance.ThemaNumber_ExamThemas].name;
                }
                else
                {
                    //End
                    ExamPaperManager.Instance.Text_QuestionNumber.GetComponent<Outline>().enabled = true;
                    if (ExamPaperManager.Instance.List_Question[(int)ExamPaperManager.Instance.Slider_Question.value].GetComponent<Questions>().IsRightAnswer)
                    {
                        ExamPaperManager.Instance.Text_QuestionNumber.text = "Q" + (ExamPaperManager.Instance.Slider_Question.value + 1) + "_" + "Right ";
                        ExamPaperManager.Instance.Text_QuestionNumber.color = ExamPaperManager.Instance.RightAnswer_ButtonColor;
                        ExamPaperManager.Instance.Text_QuestionThemaName.text = ExamPaperManager.Instance.ExamThemas[ExamPaperManager.Instance.ThemaNumber_ExamThemas].name;

                    }
                    else
                    {
                        ExamPaperManager.Instance.Text_QuestionNumber.text = "Q" + (ExamPaperManager.Instance.Slider_Question.value + 1) + "_" + "Wrong";
                        ExamPaperManager.Instance.Text_QuestionNumber.color = ExamPaperManager.Instance.WrongAnswer_ButtonColor;
                        ExamPaperManager.Instance.Text_QuestionThemaName.text = ExamPaperManager.Instance.ExamThemas[ExamPaperManager.Instance.ThemaNumber_ExamThemas].name;

                    }
                }
                ExamPaperManager.Instance.CurrentQuestionNumber = (int)ExamPaperManager.Instance.Slider_Question.value;
                ExamPaperManager.Instance.ExamPaperEnd.SetActive(false);
                for (int i = 0; i < ExamPaperManager.Instance.MaxQuestionCount; i++)
                    ExamPaperManager.Instance.List_Question[i].SetActive(false);
                ExamPaperManager.Instance.List_Question[(int)ExamPaperManager.Instance.Slider_Question.value].SetActive(true);
                ExamPaperManager.Instance.List_Question[(int)ExamPaperManager.Instance.Slider_Question.value].GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                //마지막 페이지는 결과창.
                Text_SliderInfo.text = "End";
                ExamPaperManager.Instance.CurrentQuestionNumber = (int)ExamPaperManager.Instance.Slider_Question.value;
                for (int i = 0; i < ExamPaperManager.Instance.MaxQuestionCount; i++)
                    ExamPaperManager.Instance.List_Question[i].SetActive(false);
                ExamPaperManager.Instance.ExamPaperEnd.GetComponent<CanvasGroup>().alpha = 1;
                ExamPaperManager.Instance.ExamPaperEnd.SetActive(true);
                ExamPaperManager.Instance.Text_QuestionNumber.text = "";
                ExamPaperManager.Instance.Text_QuestionThemaName.text = "";
            }
        }

        public void OnTouchDown()
        {
            Text_SliderInfo.fontSize = (int)((float)TempFontSize * 1.5f);
            Text_SliderInfo.color = Color.white;
            Text_SliderInfo.GetComponent<Outline>().enabled = true;
        }

        public void OnTouchUp()
        {
            Text_SliderInfo.fontSize = TempFontSize;
            Text_SliderInfo.color = new Color(50 / 255f, 50 / 255f, 50 / 255f);
            Text_SliderInfo.GetComponent<Outline>().enabled = false;
        }

    }
}

