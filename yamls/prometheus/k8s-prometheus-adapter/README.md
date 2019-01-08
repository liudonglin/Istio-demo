


cd /etc/kubernetes/pki/

生成密钥
(umask 077; openssl genrsa -out prometheus-adapter-serving.key 2048)

生成签名请求
openssl req -new -key prometheus-adapter-serving.key -out prometheus-adapter-serving.csr -subj "/CN=prometheus-adapter-serving"

CA签名
openssl x509 -req -in prometheus-adapter-serving.csr -CA ./ca.crt -CAkey ./ca.key -CAcreateserial -out prometheus-adapter-serving.crt -days 3650

创建secret
kubectl create secret generic cm-adapter-serving-certs --from-file=serving.crt=./prometheus-adapter-serving.crt --from-file=serving.key=./prometheus-adapter-serving.key -n prom 

kubectl api-versions
确保出现custom.metrics.k8s.io/v1beta1

kubectl get --raw "/apis/custom.metrics.k8s.io/v1beta1"