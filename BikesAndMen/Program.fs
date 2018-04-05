// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open FSharp.Data;
open FSharp.Charting;
open System;

type CsvData = CsvProvider<"C:/code/BikesAndMen/BikesAndMen/Dataset/day.csv">

let data = CsvData.Load("C:/code/BikesAndMen/BikesAndMen/Dataset/day.csv").Rows
    
let ma n (series: float seq) =
    series
    |> Seq.windowed n
    |> Seq.map (fun xs -> xs |> Seq.average)
    |> Seq.toList

[<EntryPoint>]
let main argv = 
    let sev = ma 7 [for obs in data -> (float)obs.Cnt]

    let combined = Chart.Combine[
        Chart.Line [for obs in data -> obs.Cnt]
        Chart.Line (ma 7 [for obs in data -> (float)obs.Cnt])
        Chart.Line (ma 30 [for obs in data -> (float)obs.Cnt])
    ]

    System.Windows.Forms.Application.Run(combined.ShowChart())
    let value = Console.ReadLine()
    0 // return an integer exit code
