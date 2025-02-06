#
# Aplicar todos os arquivos YAML
kubectl apply -f 01-framepack-worker-deployment.yaml
kubectl apply -f 02-framepack-worker-service.yaml
kubectl apply -f 03-framepack-worker-hpa.yaml
#
# Função para esperar um pod estar em execução
function WaitForPod {
    param (
        [string]$label
    )
    while ($true) {
        $podStatus = kubectl get pods -l app=$label -o jsonpath='{.items[*].status.phase}'
        $podStatusArray = $podStatus -split ' '
        if ($podStatusArray -contains 'Running') {
            Write-Output "Pod ${label} está em execução."
            break
        }
        else {
            Write-Output "Status atual do pod ${label}: ${podStatus}"
        }
        Write-Output "Esperando o pod ${label} estar em execução..."
        Start-Sleep -Seconds 5
    }
}
#
# Esperar os pods do Worker estarem em execução
WaitForPod -label "framepack-worker"
#
# Obter o nome do pod do Worker
$podNameWorker = kubectl get pods -l app=framepack-worker -o jsonpath='{.items[0].metadata.name}'
#
# Verificar se o pod do Worker está realmente rodando
$podStatusWorker = kubectl get pods -l app=framepack-worker -o jsonpath='{.items[*].status.phase}' -split ' '
if ($podStatusWorker -contains 'Running') {
    # Verificar o estado do contêiner dentro do pod
    $containerState = kubectl get pod $podNameWorker -o jsonpath='{.status.containerStatuses[0].state}'
    if ($containerState -contains 'running') {
        kubectl port-forward svc/framepack-worker 8080:80
    }
    elseif ($containerState -contains 'terminated') {
        Write-Output "O contêiner no pod ${podNameWorker} foi terminado."
    }
    else {
        Write-Output "Estado desconhecido do contêiner no pod ${podNameWorker}: ${containerState}"
    }
}
else {
    Write-Output "O pod ${podNameWorker} não está em execução."
}