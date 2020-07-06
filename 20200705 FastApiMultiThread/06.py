# Bruno Capuano
# simple webserver with fastapi
# run with  uvicorn 06:app -reload
# test with http://127.0.0.1:8000/items/5?q=bruno

from typing import Optional

from fastapi import FastAPI

app = FastAPI()


@app.get("/")
def read_root():
    return {"Hello": "World"}


@app.get("/items/{item_id}")
def read_item(item_id: int, q: Optional[str] = None):
    return {"item_id": item_id, "q": q}