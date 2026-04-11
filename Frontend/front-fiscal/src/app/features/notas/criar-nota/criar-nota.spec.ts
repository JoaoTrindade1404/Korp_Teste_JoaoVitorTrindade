import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CriarNota } from './criar-nota';

describe('CriarNota', () => {
  let component: CriarNota;
  let fixture: ComponentFixture<CriarNota>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CriarNota],
    }).compileComponents();

    fixture = TestBed.createComponent(CriarNota);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
