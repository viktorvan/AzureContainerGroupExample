open Farmer
open Farmer.Builders
open Farmer.Deploy
open System
open Farmer.ContainerGroup


let storage = storageAccount {
    name "tinkerpopstorage"
    sku Storage.Standard_LRS
    add_file_share_with_quota "tinkerpop-share" 10<Gb>
}

let tinkerpopContainer = 
    containerInstance {
        name "tinkerpop"
        image "tinkerpop/gremlin-server:3.4.8"
        add_public_ports [ 8182us ]
        memory 1.0<Gb>
        cpu_cores 1
        env_vars [
        ]
        add_volume_mount "tinkerpop-db" "/var/lib/tinkerpop"
    }

let tinkerpop = containerGroup {
    name "tinkerpop"
    operating_system Linux
    restart_policy AlwaysRestart
    add_instances [ tinkerpopContainer ]
    add_volumes [
        volume_mount.azureFile "tinkerpop-db" "tinkerpop-share" storage.Name.ResourceName.Value
    ]
}

// Add resources to the ARM deployment using the add_resource keyword.
// See https://compositionalit.github.io/farmer/api-overview/resources/arm/ for more details.
let deployment = arm {
    location Location.WestEurope
    add_resources [ storage; tinkerpop ]
}

printf "Generating ARM template..."
deployment |> Writer.quickWrite "output"
printfn "all done! Template written to output.json"

// Alternatively, deploy your resource group directly to Azure here.
// deployment
// |> Deploy.execute "container-group-example" Deploy.NoParameters
// |> printfn "%A"
