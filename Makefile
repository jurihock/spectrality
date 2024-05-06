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
	@cmake -DCMAKE_BUILD_TYPE=$(CFG) -S $(SRC)/Spectrality.Library -B $(DST)/Spectrality.Library
	@cmake --build $(DST)/Spectrality.Library
	@dotnet build $(SRC)/Spectrality --configuration $(CFG)
	@cp -v $(DST)/Spectrality.Library/spectrality/*.dll $(DST)/Spectrality/bin/$(CFG)

publish: build
	@dotnet publish $(SRC)/Spectrality --configuration $(CFG)
	@cp -v $(DST)/Spectrality.Library/spectrality/*.dll $(DST)/Spectrality/bin/$(CFG)/publish

run: build
	@dotnet run --project $(SRC)/Spectrality --configuration $(CFG)

clean:
	@rm -rf $(DST)
