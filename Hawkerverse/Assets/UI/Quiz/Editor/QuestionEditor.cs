using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace Lordmin
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Questions))]
    public class QuestionEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Questions questions = (Questions)target;

            //if (GUILayout.Button("Settings Exam paper"))
            //{
            //    questions.SettingExamPaper();
            //}


            Questions questions = (Questions)target;

            if (GUILayout.Button("Settings Exam paper"))
            {
                Questions[] questionss = Array.ConvertAll(targets, _t => _t as Questions);
                foreach (Questions question in questionss)
                {
                    if (question)
                        question.SettingExamPaper();
                }

            }


            //정답 범위 밖으로 정답번호를 설정했다면
            //If you set the correct answer number out of the correct answer range
            if (questions.RightAnswerNumber > questions.StringAnswers.Length || questions.RightAnswerNumber < 1)
                questions.RightAnswerNumber = questions.StringAnswers.Length;


            if (questions.AnswerCount != questions.CountChecker)
            {
                switch (questions.AnswerCount)
                {
                    case 2:
                        questions.SettingQuestion();
                        break;
                    case 3:
                        questions.SettingQuestion();
                        break;
                    case 4:
                        questions.SettingQuestion();
                        break;
                    case 5:
                        questions.SettingQuestion();
                        break;
                }
            }


            EditorUtility.SetDirty(target);
        }

    }

}
