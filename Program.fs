// Learn more about F# at http://fsharp.org

open System
open HelloRabbit.Inbound
open System.Threading

let rec wait () =
    Thread.Sleep 2000
    printfn "."
    wait ()

[<EntryPoint>]
let main argv =
    let receiveConfig: Receiver.ConnectionConfig = 
                        { UserName = "guest"
                          Password = "guest"
                          VirtualHost = "/"
                          HostName = "172.17.0.3"
                          QueueName = "test" }

    let _startReceiver1 = Receiver.start receiveConfig "receiver1"
    let _startReceiver2 = Receiver.start receiveConfig "receiver2"
    let _startReceiver3 = Receiver.start receiveConfig "receiver3"
    
    wait ()

    0 // return an integer exit code
