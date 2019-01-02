kubectl apply -f namespace.yaml

kubectl apply -f ./node_exporter/

kubectl apply -f ./prometheus/

kubectl apply -f ./kube-state-metrics/

进入k8s-prometheus-adapter执行readme中证书生成过程

kubectl apply -f ./k8s-prometheus-adapter

