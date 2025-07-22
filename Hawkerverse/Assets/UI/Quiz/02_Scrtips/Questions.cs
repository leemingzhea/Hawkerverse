using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lordmin
{
    public class Questions : MonoBehaviour
    {
        [Header("[Question Settings]")]

        [TextArea]
        public string Question;
        public Sprite QuestionImage = null;

        [Header("[Answer Settings]")]
        [Range(2, 5)]
        public int AnswerCount = 4;

        [HideInInspector]
        public int CountChecker = 0;
        private int Number = 0;

        public string[] StringAnswers;

        [Header("[RightAnswerNumber Range: 1 ~ AnswerCount]")]
        public int RightAnswerNumber = 1;

        [Space(10)]
        [Header("[IsRandomAnswer: Random Answer Number]")]
        public bool IsRandomRightAnswer = true;

        private Transform Answers;
        private Text Text_Question;
        [HideInInspector]
        public Image Image_Example, Image_Right, Image_Wrong;

        [HideInInspector]
        public int SelectAnswerNumber = 0;
        [HideInInspector]
        public bool IsRightAnswer = false;

        private void Awake()
        {
            SettingExamPaper();

            
        }

        void Start()
        {
            if (IsRandomRightAnswer)
                SettingRandomAnswer();
        }

        //Add Asset Version 1.1
        public void AddOutline()
        {
            for(int i = 0; i < Answers.childCount; i++)
            {
                if (Answers.GetChild(i).GetComponent<Outline>() == null)
                {
                    Answers.GetChild(i).gameObject.AddComponent<Outline>();
                    Answers.GetChild(i).gameObject.GetComponent<Outline>().effectColor = new Color(20/255f, 138/255f, 255/255f, 1.0f);
                    Answers.GetChild(i).gameObject.GetComponent<Outline>().effectDistance = new Vector2(5, -5);
                    Answers.GetChild(i).gameObject.GetComponent<Outline>().enabled = false;
                }
                else
                {
                    Answers.GetChild(i).gameObject.GetComponent<Outline>().enabled = false;
                }
            }
        }

        public void SettingQuestion()
        {
            if (AnswerCount != CountChecker)
            {
                CountChecker = AnswerCount;
                StringAnswers = new string[AnswerCount];

                SettingExamPaper();
            }
        }

        public void SettingExamPaper()
        {
            ExamPaperManager.Instance.ExamPaperStart.SetActive(false);
            ExamPaperManager.Instance.ExamPaperEnd.SetActive(false);

            //Set Name
            Number = transform.GetSiblingIndex() + 1;
            transform.name = "Question" + "_" + Number;

            //Set Value
            Answers = transform.Find("Answers");
            Image_Example = transform.Find("Image_Example").GetComponent<Image>();
            Text_Question = transform.Find("Text_Question").GetComponent<Text>();
            Image_Right = Text_Question.transform.Find("Image_Right").GetComponent<Image>();
            Image_Wrong = Text_Question.transform.Find("Image_Wrong").GetComponent<Image>();
            Image_Right.gameObject.SetActive(false);
            Image_Wrong.gameObject.SetActive(false);

            //Set Question Text and Image
            Text_Question.text = "Q" + Number + ". " + Question;
            Image_Example.sprite = QuestionImage;
            if (QuestionImage == null)
                Image_Example.gameObject.SetActive(false);
            else
                Image_Example.gameObject.SetActive(true);

            //Set Answer object
            for (int i = 0; i < 5; i++)
            {
                Answers.GetChild(i).gameObject.SetActive(false);
                Answers.GetChild(i).name = "Button_WrongAnswer";
            }
            for (int i = 0; i < AnswerCount; i++)
            {
                Answers.GetChild(i).gameObject.SetActive(true);
                Answers.GetChild(i).GetChild(0).GetComponent<Text>().text = (i + 1).ToString() + ". " + StringAnswers[i];
            }

            InitResetButtonColor();

            if (RightAnswerNumber <= StringAnswers.Length && RightAnswerNumber > 0)
            {
                //RightButton Set Name
                Answers.GetChild(RightAnswerNumber - 1).name = "Button_RightAnswer";
            }

            for (int i = 0; i < transform.parent.parent.childCount; i++)
            {
                for(int j = 0; j< transform.parent.parent.transform.GetChild(i).childCount; j++)
                    transform.parent.parent.transform.GetChild(i).GetChild(j).gameObject.SetActive(false);
            }

            gameObject.SetActive(true);
        }


        public void SettingRandomAnswer()
        {
            Answers = transform.Find("Answers");
            for (int i = 0; i < AnswerCount; i++)
            {
                Answers.GetChild(i).SetSiblingIndex(Random.Range(0, 3));
            }

            for (int i = 0; i < AnswerCount; i++)
            {
                string _RemoveText = Answers.GetChild(i).GetChild(0).GetComponent<Text>().text.Remove(0, 3);
                string _InsertText = _RemoveText.Insert(0, (i + 1).ToString() + ". ");
                Answers.GetChild(i).GetChild(0).GetComponent<Text>().text = _InsertText;
            }
        }


        public void Button_Answer()
        {
            if (ExamPaperManager.Instance.IsNextAnimation == false && ExamPaperManager.Instance.IsEnd == false)
            {
                SelectAnswerNumber = EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex() + 1;

                if (EventSystem.current.currentSelectedGameObject.name == "Button_RightAnswer")
                {
                    //right answer
                    Debug.Log("right answer");
                    ResetButtonColor();
                    EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = ExamPaperManager.Instance.SelectAnswer_ButtonColor;
                }
                else
                {
                    //wrong answer
                    Debug.Log("wrong answer");
                    ResetButtonColor();
                    EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = ExamPaperManager.Instance.SelectAnswer_ButtonColor;
                }

                Answers.GetChild(SelectAnswerNumber - 1).gameObject.GetComponent<Outline>().enabled = true;

                ExamPaperManager.Instance.NextQuestion();
            }
        }

        public void ResetButtonColor()
        {
            Answers = transform.Find("Answers");
            for (int i = 0; i < AnswerCount; i++)
            {
                Answers.GetChild(i).GetComponent<Image>().color = ExamPaperManager.Instance.NormalAnswer_ButtonColor;
                Answers.GetChild(i).gameObject.GetComponent<Outline>().enabled = false;
            }
        }


        public void InitResetButtonColor()
        {
            IsRightAnswer = false;
            SelectAnswerNumber = 0;
            Answers = transform.Find("Answers");
            for (int i = 0; i < AnswerCount; i++)
            {
                Answers.GetChild(i).GetComponent<Image>().color = ExamPaperManager.Instance.NormalAnswer_ButtonColor;

                Button b = Answers.GetChild(i).GetComponent<Button>();
                ColorBlock cb = Answers.GetChild(i).GetComponent<Button>().colors;
                cb.normalColor = ExamPaperManager.Instance.NormalAnswer_ButtonColor;
                cb.pressedColor = ExamPaperManager.Instance.SelectAnswer_ButtonColor;
                cb.highlightedColor = ExamPaperManager.Instance.SelectAnswer_ButtonColor;
                b.colors = cb;

                b.enabled = true;
            }

            Text_Question = transform.Find("Text_Question").GetComponent<Text>();
            Image_Right = Text_Question.transform.Find("Image_Right").GetComponent<Image>();
            Image_Wrong = Text_Question.transform.Find("Image_Wrong").GetComponent<Image>();
            Image_Right.gameObject.SetActive(false);
            Image_Wrong.gameObject.SetActive(false);

            //Add Version 1.1
            AddOutline();
        }

        //Set End Question
        public void SettingButton_RightWrongColor()
        {
            Answers = transform.Find("Answers");

            if (SelectAnswerNumber != 0) //if selected
            {
                if (Answers.GetChild(SelectAnswerNumber - 1).name == "Button_RightAnswer")
                {
                    //if Select Button is RightAnswer,
                    ExamPaperManager.Instance.CurrentScore += ExamPaperManager.Instance.MaxScore / ExamPaperManager.Instance.MaxQuestionCount;
                    ExamPaperManager.Instance.RightAnswerCount++;
                    Answers.GetChild(SelectAnswerNumber - 1).GetComponent<Image>().color = ExamPaperManager.Instance.RightAnswer_ButtonColor;
                    Image_Right.gameObject.SetActive(true);
                    IsRightAnswer = true;

                }
                else
                {
                    //if Select Button is WrongAnswer,
                    ExamPaperManager.Instance.WrongAnswerCount++;
                    Answers.GetChild(SelectAnswerNumber - 1).GetComponent<Image>().color = ExamPaperManager.Instance.WrongAnswer_ButtonColor;
                    Image_Wrong.gameObject.SetActive(true);
                    IsRightAnswer = false;
                }
            }
            else//if Not Selected
            {
                ExamPaperManager.Instance.WrongAnswerCount++;
                Image_Wrong.gameObject.SetActive(true);
                IsRightAnswer = false;
            }

            for (int i = 0; i < Answers.childCount; i++)
            {
                Answers.GetChild(i).GetComponent<Button>().enabled = false;

                //Modify Asset Version 1.1
                if (Answers.GetChild(i).name == "Button_RightAnswer")
                    Answers.GetChild(i).GetComponent<Image>().color = ExamPaperManager.Instance.RightAnswer_ButtonColor;
                else
                    Answers.GetChild(i).GetComponent<Image>().color = ExamPaperManager.Instance.WrongAnswer_ButtonColor;
            }
        }
    }
}
