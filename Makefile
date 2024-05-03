.PHONY: help build run clean

CFG = Release
DST = build
SRC = src

help:
	@echo build
	@echo run
	@echo clean

build:
	@cmake -DCMAKE_BUILD_TYPE=$(CFG) -S $(SRC)/spectrality-library -B $(DST)/spectrality-library
	@cmake --build $(DST)/spectrality-library
	@dotnet build $(SRC)/spectrality --configuration $(CFG)
	@cp $(DST)/spectrality-library/spectrality/*.dll $(DST)/spectrality/bin/$(CFG)

run:
	@dotnet run --project $(SRC)/spectrality --configuration $(CFG)

clean:
	@rm -rf $(DST)
