from pathlib import Path
from transformers import AutoTokenizer, AutoModel
from optimum.onnxruntime import ORTModelForFeatureExtraction

model_name = "sentence-transformers/distiluse-base-multilingual-cased-v2"
output_dir = Path("distiluse-base-multilingual-cased-v2-onnx")
output_dir.mkdir(parents=True, exist_ok=True)

tokenizer = AutoTokenizer.from_pretrained(model_name)
tokenizer.save_pretrained(output_dir)

model = ORTModelForFeatureExtraction.from_pretrained(
    model_name,
    export=True
)

model.save_pretrained(output_dir)

print("Export erfolgreich!")
print(f"ONNX Model:  {output_dir / 'model.onnx'}")
print(f"Tokenizer:   {output_dir / 'vocab.txt'}")
