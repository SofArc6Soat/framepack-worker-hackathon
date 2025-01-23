kubectl apply -f 01-sql-data-pvc.yaml
kubectl apply -f 02-sql-log-pvc.yaml
kubectl apply -f 03-sql-secrets-pvc.yaml
kubectl apply -f 04-quickfood-sqlserver-deployment.yaml
kubectl apply -f 05-quickfood-sqlserver-service.yaml
kubectl apply -f 06-quickfood-backend-deployment.yaml
kubectl apply -f 07-quickfood-backend-service.yaml
kubectl apply -f 08-quickfood-backend-hpa.yaml

# Esperar o pod antes de iniciar o port-forward
while ($true) {
    $podStatus = kubectl get pods -l app=quickfood-backend -o jsonpath='{.items[*].status.phase}'
    $podStatusArray = $podStatus -split ' '
    if ($podStatusArray -contains 'Running') {
        Write-Output "Pod quickfood-backend esta em execucao."
        break
    }
    else {
        Write-Output "Status atual do pod quickfood-backend: $podStatus"
    }
    Write-Output "Esperando o pod quickfood-backend estar em execucao..."
    Start-Sleep -Seconds 5
}

# Obter o nome do pod
$podName = kubectl get pods -l app=quickfood-backend -o jsonpath='{.items[0].metadata.name}'

# Verificar se o pod está realmente rodando
if ($podStatusArray -contains 'Running') {
    kubectl port-forward svc/quickfood-backend 8080:80
}
else {
    Write-Output "O pod nao esta em execucao. Verificando logs"
    kubectl describe pod $podName
    kubectl logs $podName
}
