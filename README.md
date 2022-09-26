# StepAnalysis



C# WPF application for the ability to analyze the number of steps taken over a certain period (with plotting) for different users.


https://user-images.githubusercontent.com/62507128/192224804-750710df-a86c-468e-8a69-0f38f20de272.mp4



Example input data JSON:
{
    "Rank": 5,
    "User": "Сидоров Виктор",	
    "Status": "Finished",
    "Steps": 4325
},




Example export data for all users to disk in the format JSON:

{
  "user":"Липатов Александр",
  "avg":47253,
  "max":74602,
  "min":25123,
  "usersDays":
      [
        {"day":1,"rank":1,"status":"Finished","steps":66683},
        {"day":2,"rank":1,"status":"Finished","steps":47265},
        {"day":3,"rank":21,"status":"Finished","steps":30349},
        ...
       ]
}
