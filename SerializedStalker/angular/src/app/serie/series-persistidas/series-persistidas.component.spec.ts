import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeriesPersistidasComponent } from './series-persistidas.component';

describe('SeriesPersistidasComponent', () => {
  let component: SeriesPersistidasComponent;
  let fixture: ComponentFixture<SeriesPersistidasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeriesPersistidasComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SeriesPersistidasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
