// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open FSharp.Data;
open FSharp.Charting;
open MathNet
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Double
open System;

type CsvData = CsvProvider<"C:/code/BikesAndMen/BikesAndMen/Dataset/day.csv">
type Obs = CsvData.Row
type Model = Obs -> float

let data = CsvData.Load("C:/code/BikesAndMen/BikesAndMen/Dataset/day.csv").Rows
    
let ma n (series: float seq) =
    series
    |> Seq.windowed n
    |> Seq.map (fun xs -> xs |> Seq.average)
    |> Seq.toList

let baseline = 
    let avg = data |> Seq.averageBy(fun x -> float x.Cnt)
    data |> Seq.averageBy (fun x -> abs (float x.Cnt - avg))


let model(theta0, theta1) (obs:Obs) = 
    theta0 + theta1 * (float obs.Instant)

let model0 = model(4504., 0.)
let model1 = model(6000., -4.5)


let cost(data: Obs seq) (m:Model) = 
    data 
    |> Seq.sumBy(fun x -> pown(float x.Cnt - m x) 2)
    |> sqrt

let update alpha (theta0, theta1) (obs:Obs) = 
    let y = float obs.Cnt
    let x = float obs.Instant
    let theta0' = theta0 - 2. * alpha * 1. * (theta0 + theta1*x - y)
    let theta1' = theta1 - 2. * alpha * x * (theta0 + theta1 * x - y)
    theta0', theta1'

let stochastic rate (theta0, theta1) = 
    data
    |> Seq.fold(fun(t0, t1) obs -> 
 //   printf "%.4f, %.4f \n" t0 t1
    update rate (t0, t1) obs) (theta0, theta1)

let batchUpdate rate (theta0, theta1) (data:Obs seq) =
    let updates =
        data
        |> Seq.map (update rate (theta0, theta1))
    let theta0' = updates |> Seq.averageBy fst
    let theta1' = updates |> Seq.averageBy snd
    theta0', theta1'

let batch rate iters =
    let rec search (t0,t1) i =
        if i = 0 then (t0,t1)
        else
        search (batchUpdate rate (t0,t1) data) (i-1)
    search (0.0,0.0) iters

[<EntryPoint>]
let main argv = 
    let count = [for obs in data -> obs.Cnt]

    let overallCost = cost data 
    overallCost model0 |> printf "Cost model0: %.0f \n"
    overallCost model1 |> printf "Cost model1: %.0f \n"

    let tune_rate = 
        [for r in 1..20 -> 
            (pown 0.1 r), stochastic (pown 0.1 r) (0., 0.) |> model |> overallCost]

    for i in tune_rate do
        match i with 
        | (alpha, cost) -> printf "%.15f, %.4f \n" alpha cost

    let rate = pown 0.1 8
    let model2 = model (stochastic rate (0.0, 0.0))

    let modelChart = Chart.Combine [
        Chart.Line count
        Chart.Line [for obs in data -> model2 obs ]
    ]
    
    System.Windows.Forms.Application.Run(modelChart.ShowChart())

    let sev = ma 7 [for obs in data -> (float)obs.Cnt]
    let combinedOne = Chart.Combine [
        Chart.Line [for obs in data -> obs.Cnt]
        Chart.Line [ for obs in data -> model0 obs ]
        Chart.Line [ for obs in data -> model1 obs ] ]

    let combined = Chart.Combine[
        Chart.Line [for obs in data -> obs.Cnt]
        Chart.Line (ma 7 [for obs in data -> float obs.Cnt])
        Chart.Line (ma 30 [for obs in data -> float obs.Cnt])
    ]
    let value = Console.ReadLine()
    0 // return an integer exit code
