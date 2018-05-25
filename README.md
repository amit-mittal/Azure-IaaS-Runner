# Azure-IaaS-Runner
Runner which can be run locally or as a cloud service to deploy VMs of different sizes and having variety of base images (Ubuntu, Windows Server 2016, Cent OS, Checkpoint, etc.)

## Motivation
It can be used to continuously deploy the VMs, do validations that VM has properly come up and then cleanup the resource. This is also to represent the pattern what we should be using for validating newer features.

## Other Points
Random points about the project:
- Test Suite: Collection of Tests
- It uses newer Azure SDK APIs to deploy VMs
- Secrets have been removed from the code so, the application won't work right away
- To add a new test, just add a new ARM Template and ARM parameter file
- Further monitors can be made on top of the metrics that are pushed out of the results