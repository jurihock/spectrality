.PHONY: help build publish run clean

CFG = Release
DST = build
SRC = src

help:
	@echo build
	@echo publish
	@echo run
	@echo clean

build:
	@cmake -DCMAKE_BUILD_TYPE=$(CFG) -S $(SRC)/spectrality-library -B $(DST)/spectrality-library
	@cmake --build $(DST)/spectrality-library
	@dotnet build $(SRC)/spectrality --configuration $(CFG)
	@cp -v $(DST)/spectrality-library/spectrality/*.dll $(DST)/spectrality/bin/$(CFG)

publish: build
	@dotnet publish $(SRC)/spectrality --configuration $(CFG)
	@cp -v $(DST)/spectrality-library/spectrality/*.dll $(DST)/spectrality/bin/$(CFG)/publish

run: build
	@dotnet run --project $(SRC)/spectrality --configuration $(CFG)

clean:
	@rm -rf $(DST)
