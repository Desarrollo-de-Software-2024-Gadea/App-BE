import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonitoreosComponent } from './monitoreos.component';

describe('MonitoreosComponent', () => {
  let component: MonitoreosComponent;
  let fixture: ComponentFixture<MonitoreosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MonitoreosComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MonitoreosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
