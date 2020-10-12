open Farmer
open Farmer.Builders
open Farmer.ContainerGroup


let janusContainer = 
    containerInstance {
        name "janusGraph"
        image "janusgraph/janusgraph:0.5.2"
        add_public_ports [ 8182us ]
        memory 16.0<Gb>
        cpu_cores 4
        env_vars [
            env_var "JAVA_OPTS" "-Xmx16g"
        ]
    }

let janusgraph = containerGroup {
    name "janusgraph"
    operating_system Linux
    restart_policy AlwaysRestart
    public_dns "janusgraph" [ TCP, 8182us ]
    add_instances [ janusContainer ]
}

// Add resources to the ARM deployment using the add_resource keyword.
// See https://compositionalit.github.io/farmer/api-overview/resources/arm/ for more details.
let deployment = arm {
    location Location.WestEurope
    add_resources [ janusgraph ]
}

printf "Generating ARM template..."
deployment |> Writer.quickWrite "output"
printfn "all done! Template written to output.json"

// Alternatively, deploy your resource group directly to Azure here.
// deployment
// |> Deploy.execute "container-group-example" Deploy.NoParameters
// |> printfn "%A"
