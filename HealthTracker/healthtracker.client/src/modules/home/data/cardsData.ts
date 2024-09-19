import type { ICardModel } from "../types/CardModel";

const Cards : ICardModel = [
  {
    name: "Diary",
    title: "Meals",
    description: "Users can record their meals along with calorie and nutritional composition.",
    label:'label',
    fieldValue: ['val1','val2']
  },
  {
    name: "Planner",
    title: "Trainings Planner",
    description: "Ability to create training plans, record physical activity and monitor progress.",
    label:'label',
    fieldValue: ['val1','val2']
  } ,
  {
    name: "Health",
    title: "Health Check",
    description: "Record key health indicators (weight, blood pressure, sugar levels) and visualize changes over time.",
    label:'label',
    fieldValue: ['val1','val2']
  },
  {
    name: "Goals",
    title: "Goals and Progress",
    description: "Set health and fitness goals, track progress, receive notifications of achievements.",
    label:'label',
    fieldValue: ['val1','val2']
  },
  {
    name: "Community",
    title: "Community",
    description: "A forum for users to share experiences, advice and motivational support.",
    label:'label',
    fieldValue: ['val1','val2']
  },
];

export { Cards };
