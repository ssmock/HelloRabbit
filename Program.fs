// Learn more about F# at http://fsharp.org

open System
open HelloRabbit.Inbound
open System.Threading

let rec waitForQueue startResult =
    Thread.Sleep 2000
    printfn "."
    waitForQueue startResult

[<EntryPoint>]
let main argv =
    let receiveConfig: Receiver.ConnectionConfig = 
                        { UserName = "guest"
                          Password = "guest"
                          VirtualHost = "/"
                          HostName = "172.17.0.3"
                          QueueName = "test" }

    let startReceiver1 = Receiver.start receiveConfig "receiver1"
    
    waitForQueue startReceiver1

    0 // return an integer exit code
