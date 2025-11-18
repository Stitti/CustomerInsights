from transformers import AutoTokenizer
from optimum.onnxruntime import ORTModelForSequenceClassification

name = "textattack/bert-base-uncased-MNLI"
tokenizer = AutoTokenizer.from_pretrained(name)
model = ORTModelForSequenceClassification.from_pretrained(name, export=True)

model.save_pretrained("mnli-onnx")      # writes onnx model files
tokenizer.save_pretrained("mnli-onnx")  # writes vocab.txt etc.
