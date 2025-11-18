from transformers import AutoTokenizer
from optimum.onnxruntime import ORTModelForSequenceClassification

model_id = "yiyanghkust/finbert-tone"  # 3 Klassen: neutral / positive / negative

tok = AutoTokenizer.from_pretrained(model_id)
model = ORTModelForSequenceClassification.from_pretrained(model_id, export=True)

model.save_pretrained("sentiment-onnx-3class")
tok.save_pretrained("sentiment-onnx-3class")

print("✅ exportiert nach sentiment-onnx-3class/")
